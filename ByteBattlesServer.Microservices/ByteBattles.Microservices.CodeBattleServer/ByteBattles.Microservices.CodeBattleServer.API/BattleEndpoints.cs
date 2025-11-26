using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattlesServer.Domain.Results;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.API;

public static class BattleEndpoints
{
    private static readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();

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

        // REST endpoints для управления комнатами (опционально)
        group.MapGet("/rooms", async (IMediator mediator, HttpContext http) =>
        {
            try
            {
                // Здесь можно добавить логику для получения списка комнат
                return Results.Ok(new { message = "Rooms endpoint - to be implemented" });
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
        await connectionManager.AddConnection(playerId, webSocket.GetHashCode().ToString());

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
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await ProcessMessage(playerId, message, mediator, connectionManager);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            await SendToPlayer(playerId, new { type = "error", message = ex.Message });
        }
        finally
        {
            _sockets.TryRemove(playerId, out _);
            await connectionManager.RemoveConnection(playerId, webSocket.GetHashCode().ToString());
            await LeaveAllRooms(playerId, connectionManager);
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
                        connectionManager);
                    break;
                case "SubmitCode":
                    await SubmitCode(playerId,
                        json.RootElement.GetProperty("roomId").GetGuid(),
                        json.RootElement.GetProperty("problemId").GetString(),
                        json.RootElement.GetProperty("code").GetString(),
                        mediator);
                    break;
                default:
                    await SendToPlayer(playerId, new { type = "error", message = $"Unknown message type: {type}" });
                    break;
            }
        }
        catch (JsonException ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = $"Invalid JSON format: {ex.Message}" });
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message });
        }
    }

    private static async Task CreateRoom(
        Guid playerId, 
        string roomName, 
        Guid languageId, 
        IMediator mediator, 
        IConnectionManager connectionManager)
    {
        try
        {
            var command = new CreateRoomCommand(roomName, playerId, languageId);
            var result = await mediator.Send(command);

            await connectionManager.AddUserToRoom(playerId, result.Id);
            
            await SendToPlayer(playerId, new
            {
                type = "room_created",
                roomId = result.Id,
                roomName = roomName,
                message = $"Комната '{roomName}' создана"
            });
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message });
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
            // Здесь нужно добавить команду JoinRoomCommand
            // var command = new JoinRoomCommand(roomId, playerId);
            // var result = await mediator.Send(command);
            
            await connectionManager.AddUserToRoom(playerId, roomId);
            
            await SendToPlayer(playerId, new
            {
                type = "joined_room",
                roomId = roomId,
                message = $"Вы присоединились к комнате {roomId}"
            });

            // Уведомляем других участников комнаты
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "player_joined",
                playerId = playerId.ToString(),
                roomId = roomId
            });
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message });
        }
    }

    private static async Task LeaveRoom(
        Guid playerId, 
        Guid roomId, 
        IConnectionManager connectionManager)
    {
        try
        {
            await connectionManager.RemoveUserFromRoom(playerId, roomId);
            
            await SendToPlayer(playerId, new
            {
                type = "left_room",
                roomId = roomId,
                message = $"Вы покинули комнату {roomId}"
            });

            // Уведомляем других участников комнаты
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "player_left",
                playerId = playerId.ToString(),
                roomId = roomId
            });
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message });
        }
    }

    private static async Task SubmitCode(
        Guid playerId, 
        Guid roomId, 
        string problemId, 
        string code, 
        IMediator mediator)
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
            });

            // Уведомляем других участников комнаты
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "code_submitted_by_player",
                playerId = playerId.ToString(),
                problemId = problemId,
                roomId = roomId
            });

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
            });
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message });
        }
    }

    private static async Task LeaveAllRooms(Guid playerId, IConnectionManager connectionManager)
    {
        var userRooms = await connectionManager.GetUserRooms(playerId);
        foreach (var roomId in userRooms)
        {
            await connectionManager.RemoveUserFromRoom(playerId, roomId);
            await BroadcastToRoom(playerId, roomId, new 
            { 
                type = "player_disconnected", 
                playerId = playerId.ToString() 
            });
        }
    }

    private static async Task SendToPlayer(Guid playerId, object message)
    {
        if (_sockets.TryGetValue(playerId, out var webSocket) && webSocket.State == WebSocketState.Open)
        {
            await SendMessage(webSocket, message);
        }
    }

    private static async Task BroadcastToRoom(Guid senderId, Guid roomId, object message)
    {
        // Здесь нужно реализовать логику рассылки сообщений всем участникам комнаты
        // Для этого нужно хранить информацию о том, какие пользователи в каких комнатах
        var roomParticipants = await GetRoomParticipants(roomId);
        foreach (var participantId in roomParticipants)
        {
            if (participantId != senderId)
            {
                await SendToPlayer(participantId, message);
            }
        }
    }

    private static async Task<List<Guid>> GetRoomParticipants(Guid roomId)
    {
        // Временная реализация - нужно интегрировать с реальной логикой комнат
        return new List<Guid>();
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
            // Для тестирования - генерируем случайный ID
            // В реальном приложении здесь должна быть аутентификация
            var testUserId = Guid.NewGuid();
            Console.WriteLine($"Generated test user ID: {testUserId}");
            return testUserId;
        }

        return Guid.Parse(userIdClaim);
    }
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
        return Task.CompletedTask;
    }

    public Task RemoveConnection(Guid userId, string connectionId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            connections.Remove(connectionId);
            if (connections.Count == 0)
                _userConnections.TryRemove(userId, out _);
        }
        return Task.CompletedTask;
    }

    public Task AddUserToRoom(Guid userId, Guid roomId)
    {
        _userRooms.AddOrUpdate(userId,
            new List<Guid> { roomId },
            (_, rooms) =>
            {
                rooms.Add(roomId);
                return rooms;
            });
        return Task.CompletedTask;
    }

    public Task RemoveUserFromRoom(Guid userId, Guid roomId)
    {
        if (_userRooms.TryGetValue(userId, out var rooms))
        {
            rooms.Remove(roomId);
            if (rooms.Count == 0)
                _userRooms.TryRemove(userId, out _);
        }
        return Task.CompletedTask;
    }

    public Task<List<Guid>> GetUserRooms(Guid userId)
    {
        if (_userRooms.TryGetValue(userId, out var rooms))
            return Task.FromResult(rooms);

        return Task.FromResult(new List<Guid>());
    }
}