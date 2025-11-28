using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.API;

public static class BattleEndpoints
{
    private static readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();
    private static readonly ConcurrentDictionary<Guid, List<Guid>> _roomParticipants = new();
    private static readonly ConcurrentDictionary<Guid, DateTime> _lastActivity = new();
    private static readonly Timer _cleanupTimer;

    static BattleEndpoints()
    {
        // Таймер для очистки неактивных соединений (каждые 30 секунд)
        _cleanupTimer = new Timer(CleanupInactiveConnections, null, 
            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    public static void MapBattleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/battle")
            .WithTags("Battle");

        // WebSocket endpoint для битв
        group.MapGet("/", async (HttpContext http, IMediator mediator, IConnectionManager connectionManager) =>
        {
            if (http.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await http.WebSockets.AcceptWebSocketAsync();
                await HandleWebSocketConnection(webSocket, mediator, connectionManager, http);
            }
            else
            {
                http.Response.StatusCode = 400;
                await http.Response.WriteAsync("WebSocket connection required");
            }
        })
        .WithName("BattleWebSocket")
        .WithSummary("WebSocket соединение для битв")
        .WithDescription("Устанавливает WebSocket соединение для участия в программистских битвах")
        .Produces(StatusCodes.Status101SwitchingProtocols)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        // REST endpoints для управления комнатами
        group.MapGet("/rooms", async (IMediator mediator, HttpContext http) =>
        {
            try
            {
                var activeRooms = _roomParticipants.Where(r => r.Value.Count > 0)
                    .Select(r => new { 
                        RoomId = r.Key, 
                        ParticipantCount = r.Value.Count,
                        Participants = r.Value
                    })
                    .ToList();
                
                return Results.Ok(new { 
                    rooms = activeRooms,
                    totalConnections = _sockets.Count,
                    activeUsers = _lastActivity.Count
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred: {ex.Message}");
            }
        })
        .WithName("GetBattleRooms")
        .WithSummary("Получение списка комнат для битв")
        .WithDescription("Возвращает список активных комнат для программистских битв")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
    }

    private static async Task HandleWebSocketConnection(
        WebSocket webSocket, 
        IMediator mediator, 
        IConnectionManager connectionManager, 
        HttpContext httpContext)
    {
        var playerId = GetUserIdFromContext(httpContext);
        _sockets[playerId] = webSocket;
        _lastActivity[playerId] = DateTime.UtcNow; // Записываем время подключения
        await connectionManager.AddConnection(playerId, webSocket.GetHashCode().ToString());

        Console.WriteLine($"Player {playerId} connected. Total connections: {_sockets.Count}");

        try
        {
            await SendMessage(webSocket, new
            {
                type = "connected",
                playerId = playerId.ToString(),
                message = "Подключение к серверу битв установлено"
            });

            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    // Обновляем время последней активности
                    _lastActivity[playerId] = DateTime.UtcNow;
                    
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await ProcessMessage(playerId, message, mediator, connectionManager);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await HandleDisconnection(playerId, connectionManager, "Normal closure", mediator);
                    break;
                }
            }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            await HandleDisconnection(playerId, connectionManager, "Connection closed prematurely", mediator);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error for player {playerId}: {ex.Message}");
            await HandleDisconnection(playerId, connectionManager, $"Error: {ex.Message}", mediator);
        }
        finally
        {
            CleanupPlayer(playerId, connectionManager);
        }
    }

    private static async Task HandleDisconnection(Guid playerId, IConnectionManager connectionManager, string reason, IMediator mediator)
    {
        Console.WriteLine($"Player {playerId} disconnected. Reason: {reason}");
        await LeaveAllRooms(playerId, connectionManager,  mediator);
        
        // Уведомляем о дисконнекте
        await BroadcastToAllRooms(playerId, new 
        { 
            type = "player_disconnected", 
            playerId = playerId.ToString(),
            reason = reason
        },mediator);
    }

    private static void CleanupPlayer(Guid playerId, IConnectionManager connectionManager)
    {
        _sockets.TryRemove(playerId, out _);
        _lastActivity.TryRemove(playerId, out _);
        connectionManager.RemoveConnection(playerId, "cleanup").Wait();
        
        Console.WriteLine($"Player {playerId} cleaned up. Remaining connections: {_sockets.Count}");
    }

    private static async void CleanupInactiveConnections(object state)
    {
        try
        {
            var now = DateTime.UtcNow;
            var timeout = TimeSpan.FromMinutes(5); // 5 минут неактивности
            var inactivePlayers = _lastActivity
                .Where(x => now - x.Value > timeout)
                .Select(x => x.Key)
                .ToList();

            foreach (var playerId in inactivePlayers)
            {
                Console.WriteLine($"Cleaning up inactive player: {playerId}");
                if (_sockets.TryGetValue(playerId, out var socket))
                {
                    try
                    {
                        await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                            "Inactive timeout", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error closing inactive socket for {playerId}: {ex.Message}");
                    }
                }
                CleanupPlayer(playerId, new ConnectionManager());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in cleanup timer: {ex.Message}");
        }
    }

    // Добавляем ping/pong для поддержания соединения
    private static async Task SendPing(WebSocket webSocket)
    {
        try
        {
            if (webSocket.State == WebSocketState.Open)
            {
                var pingMessage = new { type = "ping", timestamp = DateTime.UtcNow };
                await SendMessage(webSocket, pingMessage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending ping: {ex.Message}");
        }
    }

    private static async Task ProcessMessage(
        Guid playerId, 
        string message, 
        IMediator mediator, 
        IConnectionManager connectionManager)
    {
        try
        {
            var json = JsonDocument.Parse(message);
            var type = json.RootElement.GetProperty("type").GetString();

            switch (type)
            {
                case "CreateRoom":
                    await CreateRoom(playerId, 
                        json.RootElement.GetProperty("roomName").GetString(),
                        json.RootElement.GetProperty("languageId").GetGuid(),
                        json.RootElement.GetProperty("difficulty").GetString(),
                        mediator, 
                        connectionManager);
                    break;
                case "JoinRoom":
                    await JoinRoom(playerId, 
                        json.RootElement.GetProperty("roomId").GetGuid(),
                        mediator, 
                        connectionManager);
                    break;
                case "LeaveRoom":
                    await LeaveRoom(playerId,
                        json.RootElement.GetProperty("roomId").GetGuid(),
                        connectionManager,
                        mediator);
                    break;
                case "SubmitCode":
                    await SubmitCode(playerId,
                        json.RootElement.GetProperty("roomId").GetGuid(),
                        json.RootElement.GetProperty("problemId").GetString(),
                        json.RootElement.GetProperty("code").GetString(),
                        mediator,
                        connectionManager);
                    break;
                case "pong": // Обработка ответа на ping
                    _lastActivity[playerId] = DateTime.UtcNow;
                    break;
                default:
                    await SendToPlayer(playerId, new { type = "error", message = $"Unknown message type: {type}" },mediator);
                    break;
            }
        }
        catch (JsonException ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = $"Invalid JSON format: {ex.Message}" },mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
        }
    }

    // Остальные методы (CreateRoom, JoinRoom, LeaveRoom, SubmitCode) остаются без изменений
   private static async Task CreateRoom(
        Guid playerId, 
        string roomName, 
        Guid languageId, 
        string difficultyString,
        IMediator mediator, 
        IConnectionManager connectionManager)
    {
        try
        {
            if (!Enum.TryParse<Difficulty>(difficultyString, true, out var difficulty))
            {
                await SendToPlayer(playerId, new { 
                    type = "error", 
                    message = "Invalid difficulty value. Use: Easy, Medium, or Hard" 
                },mediator);
                return;
            }

            var command = new CreateRoomCommand(roomName, playerId, languageId, difficulty);
            var result = await mediator.Send(command);

            await connectionManager.AddUserToRoom(playerId, result.Id);
            
            // Добавляем пользователя в список участников комнаты
            _roomParticipants.AddOrUpdate(result.Id,
                new List<Guid> { playerId },
                (_, participants) =>
                {
                    participants.Add(playerId);
                    return participants;
                });

            await SendToPlayer(playerId, new
            {
                type = "room_created",
                roomId = result.Id,
                roomName = roomName,
                difficulty = difficulty.ToString(),
                languageId = languageId,
                message = $"Комната '{roomName}' создана"
            },mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
        }
    }

    private static async Task JoinRoom(
        Guid playerId, 
        Guid roomId, 
        IMediator mediator, 
        IConnectionManager connectionManager)
    {
        try
        {
            // Проверяем существование комнаты
            if (!_roomParticipants.ContainsKey(roomId))
            {
                await SendToPlayer(playerId, new { 
                    type = "error", 
                    message = $"Room {roomId} not found" 
                },mediator);
                return;
            }

            // Здесь можно добавить команду JoinRoomCommand
             var command = new JoinRoomCommand(roomId, playerId);
             var result = await mediator.Send(command);
            
            await connectionManager.AddUserToRoom(playerId, roomId);
            
            // Добавляем пользователя в список участников комнаты
            _roomParticipants.AddOrUpdate(roomId,
                new List<Guid> { playerId },
                (_, participants) =>
                {
                    if (!participants.Contains(playerId))
                        participants.Add(playerId);
                    return participants;
                });

            await SendToPlayer(playerId, new
            {
                type = "joined_room",
                roomId = roomId,
                message = $"Вы присоединились к комнате {roomId}",
                participants = _roomParticipants[roomId].Count
            },mediator);

            // Уведомляем других участников комнаты
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "player_joined",
                playerId = playerId.ToString(),
                roomId = roomId,
                participants = _roomParticipants[roomId].Count
            },mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
        }
    }

    private static async Task LeaveRoom(
        Guid playerId, 
        Guid roomId, 
        IConnectionManager connectionManager,
        IMediator mediator)
    {
        try
        {
            await connectionManager.RemoveUserFromRoom(playerId, roomId);
            
            // Удаляем пользователя из списка участников комнаты
            if (_roomParticipants.TryGetValue(roomId, out var participants))
            {
                participants.Remove(playerId);
                if (participants.Count == 0)
                    _roomParticipants.TryRemove(roomId, out _);
            }
            var command = new LeaveRoomCommand(roomId, playerId);
            await mediator.Send(command);
            await SendToPlayer(playerId, new
            {
                type = "left_room",
                roomId = roomId,
                message = $"Вы покинули комнату {roomId}"
            },mediator);

            // Уведомляем других участников комнаты
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "player_left",
                playerId = playerId.ToString(),
                roomId = roomId,
                participants = participants?.Count ?? 0
            },mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
        }
    }

    private static async Task SubmitCode(
        Guid playerId, 
        Guid roomId, 
        string problemId, 
        string code, 
        IMediator mediator,
        IConnectionManager connectionManager) // добавлен параметр
    {
        try
        {
            // Здесь нужно добавить команду SubmitCodeCommand
            // var command = new SubmitCodeCommand(roomId, playerId, problemId, code);
            // var result = await mediator.Send(command);
            
            await SendToPlayer(playerId, new
            {
                type = "code_submitted",
                roomId = roomId,
                problemId = problemId,
                message = "Код отправлен на проверку"
            },mediator);

            // Уведомляем других участников комнаты
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "code_submitted_by_player",
                playerId = playerId.ToString(),
                problemId = problemId,
                roomId = roomId
            },mediator);

            // Имитация результата проверки
            await Task.Delay(1000);
            await SendToPlayer(playerId, new
            {
                type = "code_result",
                roomId = roomId,
                problemId = problemId,
                result = new
                {
                    status = "completed",
                    passedTests = 5,
                    totalTests = 5,
                    executionTime = 150
                }
            },mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
        }
    }

   

    private static async Task LeaveAllRooms(Guid playerId, IConnectionManager connectionManager,   IMediator mediator)
    {
        var userRooms = await connectionManager.GetUserRooms(playerId);
        foreach (var roomId in userRooms)
        {
            await connectionManager.RemoveUserFromRoom(playerId, roomId);
         
            var command = new LeaveRoomCommand(roomId, playerId);
            await mediator.Send(command);

            
            if (_roomParticipants.TryGetValue(roomId, out var participants))
            {
                
                participants.Remove(playerId);
                if (participants.Count == 0)
                {
                    _roomParticipants.TryRemove(roomId, out _);
                    Console.WriteLine($"Room {roomId} is now empty and removed");
                }
                else
                {
                    // Уведомляем оставшихся участников
                    await BroadcastToRoom(playerId, roomId, new 
                    { 
                        type = "player_disconnected", 
                        playerId = playerId.ToString(),
                        participants = participants.Count
                    },mediator);
                }
            }
        }
        
        Console.WriteLine($"Player {playerId} left all rooms");
    }

    private static async Task BroadcastToAllRooms(Guid senderId, object message,   IMediator mediator)
    {
        var userRooms = await new ConnectionManager().GetUserRooms(senderId);
        foreach (var roomId in userRooms)
        {
            await BroadcastToRoom(senderId, roomId, message,mediator);
        }
    }

    private static async Task SendToPlayer(Guid playerId, object message,   IMediator mediator)
    {
        if (_sockets.TryGetValue(playerId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            try
            {
                await SendMessage(webSocket, message);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket exception when sending to {playerId}: {ex.Message}");
                await HandleDisconnection(playerId, new ConnectionManager(), "Send failed", mediator);
            }
        }
    }

    private static async Task BroadcastToRoom(Guid senderId, Guid roomId, object message, IMediator mediator)
    {
        if (_roomParticipants.TryGetValue(roomId, out var participants))
        {
            var tasks = participants
                .Where(participantId => participantId != senderId)
                .Select(participantId => SendToPlayer(participantId, message, mediator))
                .ToList();

            await Task.WhenAll(tasks);
        }
    }

    private static async Task SendMessage(WebSocket webSocket, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), 
            WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private static Guid GetUserIdFromContext(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst("sub")?.Value
                          ?? context.User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            var testUserId = Guid.NewGuid();
            Console.WriteLine($"Generated test user ID: {testUserId}");
            return testUserId;
        }

        return Guid.Parse(userIdClaim);
    }
}

// Обновленный ConnectionManager
public class ConnectionManager : IConnectionManager
{
    private readonly ConcurrentDictionary<Guid, List<string>> _userConnections = new();
    private readonly ConcurrentDictionary<Guid, List<Guid>> _userRooms = new();

    public Task AddConnection(Guid userId, string connectionId)
    {
        _userConnections.AddOrUpdate(userId,
            new List<string> { connectionId },
            (_, connections) =>
            {
                connections.Add(connectionId);
                return connections;
            });
        
        Console.WriteLine($"Connection added for user {userId}. Total: {_userConnections.Count}");
        return Task.CompletedTask;
    }

    public Task RemoveConnection(Guid userId, string connectionId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            connections.Remove(connectionId);
            if (connections.Count == 0)
            {
                _userConnections.TryRemove(userId, out _);
                Console.WriteLine($"All connections removed for user {userId}");
            }
        }
        return Task.CompletedTask;
    }

    public Task AddUserToRoom(Guid userId, Guid roomId)
    {
        _userRooms.AddOrUpdate(userId,
            new List<Guid> { roomId },
            (_, rooms) =>
            {
                if (!rooms.Contains(roomId))
                    rooms.Add(roomId);
                return rooms;
            });
        
        Console.WriteLine($"User {userId} added to room {roomId}");
        return Task.CompletedTask;
    }

    public Task RemoveUserFromRoom(Guid userId, Guid roomId)
    {
        if (_userRooms.TryGetValue(userId, out var rooms))
        {
            rooms.Remove(roomId);
            if (rooms.Count == 0)
            {
                _userRooms.TryRemove(userId, out _);
                Console.WriteLine($"User {userId} removed from all rooms");
            }
            else
            {
                Console.WriteLine($"User {userId} removed from room {roomId}");
            }
        }
        return Task.CompletedTask;
    }

    public Task<List<Guid>> GetUserRooms(Guid userId)
    {
        if (_userRooms.TryGetValue(userId, out var rooms))
            return Task.FromResult(rooms);

        return Task.FromResult(new List<Guid>());
    }

    // Метод для получения всех подключений (для администрирования)
    public ConcurrentDictionary<Guid, List<string>> GetAllConnections() => _userConnections;
}
// Интерфейсы и классы для управления подключениями
public interface IConnectionManager
{
    Task AddConnection(Guid userId, string connectionId);
    Task RemoveConnection(Guid userId, string connectionId);
    Task AddUserToRoom(Guid userId, Guid roomId);
    Task RemoveUserFromRoom(Guid userId, Guid roomId);
    Task<List<Guid>> GetUserRooms(Guid userId);
}

// public class ConnectionManager : IConnectionManager
// {
//     private readonly ConcurrentDictionary<Guid, List<string>> _userConnections = new();
//     private readonly ConcurrentDictionary<Guid, List<Guid>> _userRooms = new();
//
//     public Task AddConnection(Guid userId, string connectionId)
//     {
//         _userConnections.AddOrUpdate(userId,
//             new List<string> { connectionId },
//             (_, connections) =>
//             {
//                 connections.Add(connectionId);
//                 return connections;
//             });
//         return Task.CompletedTask;
//     }
//
//     public Task RemoveConnection(Guid userId, string connectionId)
//     {
//         if (_userConnections.TryGetValue(userId, out var connections))
//         {
//             connections.Remove(connectionId);
//             if (connections.Count == 0)
//                 _userConnections.TryRemove(userId, out _);
//         }
//         return Task.CompletedTask;
//     }
//
//     public Task AddUserToRoom(Guid userId, Guid roomId)
//     {
//         _userRooms.AddOrUpdate(userId,
//             new List<Guid> { roomId },
//             (_, rooms) =>
//             {
//                 if (!rooms.Contains(roomId))
//                     rooms.Add(roomId);
//                 return rooms;
//             });
//         return Task.CompletedTask;
//     }
//
//     public Task RemoveUserFromRoom(Guid userId, Guid roomId)
//     {
//         if (_userRooms.TryGetValue(userId, out var rooms))
//         {
//             rooms.Remove(roomId);
//             if (rooms.Count == 0)
//                 _userRooms.TryRemove(userId, out _);
//         }
//         return Task.CompletedTask;
//     }
//
//     public Task<List<Guid>> GetUserRooms(Guid userId)
//     {
//         if (_userRooms.TryGetValue(userId, out var rooms))
//             return Task.FromResult(rooms);
//
//         return Task.FromResult(new List<Guid>());
//     }
// }