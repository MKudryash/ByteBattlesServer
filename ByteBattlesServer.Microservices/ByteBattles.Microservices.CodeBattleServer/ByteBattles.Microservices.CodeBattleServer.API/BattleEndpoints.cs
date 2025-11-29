// using System.Collections.Concurrent;
// using System.Net.WebSockets;
// using System.Security.Claims;
// using System.Text;
// using System.Text.Json;
// using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
// using ByteBattlesServer.Domain.Results;
// using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
// using MediatR;
//
// namespace ByteBattles.Microservices.CodeBattleServer.API;
//
// public static class BattleEndpoints
// {
//     private static readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();
//     private static readonly ConcurrentDictionary<Guid, List<Guid>> _roomParticipants = new();
//     private static readonly ConcurrentDictionary<Guid, DateTime> _lastActivity = new();
//     private static readonly Timer _cleanupTimer;
//
//     static BattleEndpoints()
//     {
//         // Таймер для очистки неактивных соединений (каждые 30 секунд)
//         _cleanupTimer = new Timer(CleanupInactiveConnections, null, 
//             TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
//     }
//
//     public static void MapBattleEndpoints(this IEndpointRouteBuilder routes)
//     {
//         var group = routes.MapGroup("/api/battle")
//             .WithTags("Battle");
//
//         // WebSocket endpoint для битв
//         group.MapGet("/", async (HttpContext http, IMediator mediator, IConnectionManager connectionManager) =>
//         {
//             if (http.WebSockets.IsWebSocketRequest)
//             {
//                 var webSocket = await http.WebSockets.AcceptWebSocketAsync();
//                 await HandleWebSocketConnection(webSocket, mediator, connectionManager, http);
//             }
//             else
//             {
//                 http.Response.StatusCode = 400;
//                 await http.Response.WriteAsync("WebSocket connection required");
//             }
//         })
//         .WithName("BattleWebSocket")
//         .WithSummary("WebSocket соединение для битв")
//         .WithDescription("Устанавливает WebSocket соединение для участия в программистских битвах")
//         .Produces(StatusCodes.Status101SwitchingProtocols)
//         .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
//
//         // REST endpoints для управления комнатами
//         group.MapGet("/rooms", async (IMediator mediator, HttpContext http) =>
//         {
//             try
//             {
//                 var activeRooms = _roomParticipants.Where(r => r.Value.Count > 0)
//                     .Select(r => new { 
//                         RoomId = r.Key, 
//                         ParticipantCount = r.Value.Count,
//                         Participants = r.Value
//                     })
//                     .ToList();
//                 
//                 return Results.Ok(new { 
//                     rooms = activeRooms,
//                     totalConnections = _sockets.Count,
//                     activeUsers = _lastActivity.Count
//                 });
//             }
//             catch (Exception ex)
//             {
//                 return Results.Problem($"An error occurred: {ex.Message}");
//             }
//         })
//         .WithName("GetBattleRooms")
//         .WithSummary("Получение списка комнат для битв")
//         .WithDescription("Возвращает список активных комнат для программистских битв")
//         .Produces(StatusCodes.Status200OK)
//         .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);
//     }
//
//     private static async Task HandleWebSocketConnection(
//         WebSocket webSocket, 
//         IMediator mediator, 
//         IConnectionManager connectionManager, 
//         HttpContext httpContext)
//     {
//         var playerId = GetUserIdFromContext(httpContext);
//         _sockets[playerId] = webSocket;
//         _lastActivity[playerId] = DateTime.UtcNow; // Записываем время подключения
//         await connectionManager.AddConnection(playerId, webSocket.GetHashCode().ToString());
//
//         Console.WriteLine($"Player {playerId} connected. Total connections: {_sockets.Count}");
//
//         try
//         {
//             await SendMessage(webSocket, new
//             {
//                 type = "connected",
//                 playerId = playerId.ToString(),
//                 message = "Подключение к серверу битв установлено"
//             });
//
//             var buffer = new byte[1024 * 4];
//             while (webSocket.State == WebSocketState.Open)
//             {
//                 var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
//
//                 if (result.MessageType == WebSocketMessageType.Text)
//                 {
//                     // Обновляем время последней активности
//                     _lastActivity[playerId] = DateTime.UtcNow;
//                     
//                     var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                     await ProcessMessage(playerId, message, mediator, connectionManager);
//                 }
//                 else if (result.MessageType == WebSocketMessageType.Close)
//                 {
//                     await HandleDisconnection(playerId, connectionManager, "Normal closure", mediator);
//                     break;
//                 }
//             }
//         }
//         catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
//         {
//             await HandleDisconnection(playerId, connectionManager, "Connection closed prematurely", mediator);
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"WebSocket error for player {playerId}: {ex.Message}");
//             await HandleDisconnection(playerId, connectionManager, $"Error: {ex.Message}", mediator);
//         }
//         finally
//         {
//             CleanupPlayer(playerId, connectionManager);
//         }
//     }
//
//     private static async Task HandleDisconnection(Guid playerId, IConnectionManager connectionManager, string reason, IMediator mediator)
//     {
//         Console.WriteLine($"Player {playerId} disconnected. Reason: {reason}");
//         await LeaveAllRooms(playerId, connectionManager,  mediator);
//         
//         // Уведомляем о дисконнекте
//         await BroadcastToAllRooms(playerId, new 
//         { 
//             type = "player_disconnected", 
//             playerId = playerId.ToString(),
//             reason = reason
//         },mediator);
//     }
//
//     private static void CleanupPlayer(Guid playerId, IConnectionManager connectionManager)
//     {
//         _sockets.TryRemove(playerId, out _);
//         _lastActivity.TryRemove(playerId, out _);
//         connectionManager.RemoveConnection(playerId, "cleanup").Wait();
//         
//         Console.WriteLine($"Player {playerId} cleaned up. Remaining connections: {_sockets.Count}");
//     }
//
//     private static async void CleanupInactiveConnections(object state)
//     {
//         try
//         {
//             var now = DateTime.UtcNow;
//             var timeout = TimeSpan.FromMinutes(5); // 5 минут неактивности
//             var inactivePlayers = _lastActivity
//                 .Where(x => now - x.Value > timeout)
//                 .Select(x => x.Key)
//                 .ToList();
//
//             foreach (var playerId in inactivePlayers)
//             {
//                 Console.WriteLine($"Cleaning up inactive player: {playerId}");
//                 if (_sockets.TryGetValue(playerId, out var socket))
//                 {
//                     try
//                     {
//                         await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
//                             "Inactive timeout", CancellationToken.None);
//                     }
//                     catch (Exception ex)
//                     {
//                         Console.WriteLine($"Error closing inactive socket for {playerId}: {ex.Message}");
//                     }
//                 }
//                 CleanupPlayer(playerId, new ConnectionManager());
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error in cleanup timer: {ex.Message}");
//         }
//     }
//
//     // Добавляем ping/pong для поддержания соединения
//     private static async Task SendPing(WebSocket webSocket)
//     {
//         try
//         {
//             if (webSocket.State == WebSocketState.Open)
//             {
//                 var pingMessage = new { type = "ping", timestamp = DateTime.UtcNow };
//                 await SendMessage(webSocket, pingMessage);
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Error sending ping: {ex.Message}");
//         }
//     }
//
//     private static async Task ProcessMessage(
//         Guid playerId, 
//         string message, 
//         IMediator mediator, 
//         IConnectionManager connectionManager)
//     {
//         try
//         {
//             var json = JsonDocument.Parse(message);
//             var type = json.RootElement.GetProperty("type").GetString();
//
//             switch (type)
//             {
//                 case "CreateRoom":
//                     await CreateRoom(playerId, 
//                         json.RootElement.GetProperty("roomName").GetString(),
//                         json.RootElement.GetProperty("languageId").GetGuid(),
//                         json.RootElement.GetProperty("difficulty").GetString(),
//                         mediator, 
//                         connectionManager);
//                     break;
//                 case "JoinRoom":
//                     await JoinRoom(playerId, 
//                         json.RootElement.GetProperty("roomId").GetGuid(),
//                         mediator, 
//                         connectionManager);
//                     break;
//                 case "LeaveRoom":
//                     await LeaveRoom(playerId,
//                         json.RootElement.GetProperty("roomId").GetGuid(),
//                         connectionManager,
//                         mediator);
//                     break;
//                 case "SubmitCode":
//                     await SubmitCode(playerId,
//                         json.RootElement.GetProperty("roomId").GetGuid(),
//                         json.RootElement.GetProperty("problemId").GetString(),
//                         json.RootElement.GetProperty("code").GetString(),
//                         mediator,
//                         connectionManager);
//                     break;
//                 case "pong": // Обработка ответа на ping
//                     _lastActivity[playerId] = DateTime.UtcNow;
//                     break;
//                 default:
//                     await SendToPlayer(playerId, new { type = "error", message = $"Unknown message type: {type}" },mediator);
//                     break;
//             }
//         }
//         catch (JsonException ex)
//         {
//             await SendToPlayer(playerId, new { type = "error", message = $"Invalid JSON format: {ex.Message}" },mediator);
//         }
//         catch (Exception ex)
//         {
//             await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
//         }
//     }
//
//     // Остальные методы (CreateRoom, JoinRoom, LeaveRoom, SubmitCode) остаются без изменений
//    private static async Task CreateRoom(
//         Guid playerId, 
//         string roomName, 
//         Guid languageId, 
//         string difficultyString,
//         IMediator mediator, 
//         IConnectionManager connectionManager)
//     {
//         try
//         {
//             if (!Enum.TryParse<Difficulty>(difficultyString, true, out var difficulty))
//             {
//                 await SendToPlayer(playerId, new { 
//                     type = "error", 
//                     message = "Invalid difficulty value. Use: Easy, Medium, or Hard" 
//                 },mediator);
//                 return;
//             }
//
//             var command = new CreateRoomCommand(roomName, playerId, languageId, difficulty);
//             var result = await mediator.Send(command);
//
//             await connectionManager.AddUserToRoom(playerId, result.Id);
//             
//             // Добавляем пользователя в список участников комнаты
//             _roomParticipants.AddOrUpdate(result.Id,
//                 new List<Guid> { playerId },
//                 (_, participants) =>
//                 {
//                     participants.Add(playerId);
//                     return participants;
//                 });
//
//             await SendToPlayer(playerId, new
//             {
//                 type = "room_created",
//                 roomId = result.Id,
//                 roomName = roomName,
//                 difficulty = difficulty.ToString(),
//                 languageId = languageId,
//                 message = $"Комната '{roomName}' создана"
//             },mediator);
//         }
//         catch (Exception ex)
//         {
//             await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
//         }
//     }
//
//     private static async Task JoinRoom(
//         Guid playerId, 
//         Guid roomId, 
//         IMediator mediator, 
//         IConnectionManager connectionManager)
//     {
//         try
//         {
//             // Проверяем существование комнаты
//             if (!_roomParticipants.ContainsKey(roomId))
//             {
//                 await SendToPlayer(playerId, new { 
//                     type = "error", 
//                     message = $"Room {roomId} not found" 
//                 },mediator);
//                 return;
//             }
//
//             // Здесь можно добавить команду JoinRoomCommand
//              var command = new JoinRoomCommand(roomId, playerId);
//              var result = await mediator.Send(command);
//             
//             await connectionManager.AddUserToRoom(playerId, roomId);
//             
//             // Добавляем пользователя в список участников комнаты
//             _roomParticipants.AddOrUpdate(roomId,
//                 new List<Guid> { playerId },
//                 (_, participants) =>
//                 {
//                     if (!participants.Contains(playerId))
//                         participants.Add(playerId);
//                     return participants;
//                 });
//
//             await SendToPlayer(playerId, new
//             {
//                 type = "joined_room",
//                 roomId = roomId,
//                 message = $"Вы присоединились к комнате {roomId}",
//                 participants = _roomParticipants[roomId].Count
//             },mediator);
//
//             // Уведомляем других участников комнаты
//             await BroadcastToRoom(playerId, roomId, new
//             {
//                 type = "player_joined",
//                 playerId = playerId.ToString(),
//                 roomId = roomId,
//                 participants = _roomParticipants[roomId].Count
//             },mediator);
//         }
//         catch (Exception ex)
//         {
//             await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
//         }
//     }
//
//     private static async Task LeaveRoom(
//         Guid playerId, 
//         Guid roomId, 
//         IConnectionManager connectionManager,
//         IMediator mediator)
//     {
//         try
//         {
//             await connectionManager.RemoveUserFromRoom(playerId, roomId);
//             
//             // Удаляем пользователя из списка участников комнаты
//             if (_roomParticipants.TryGetValue(roomId, out var participants))
//             {
//                 participants.Remove(playerId);
//                 if (participants.Count == 0)
//                     _roomParticipants.TryRemove(roomId, out _);
//             }
//             var command = new LeaveRoomCommand(roomId, playerId);
//             await mediator.Send(command);
//             await SendToPlayer(playerId, new
//             {
//                 type = "left_room",
//                 roomId = roomId,
//                 message = $"Вы покинули комнату {roomId}"
//             },mediator);
//
//             // Уведомляем других участников комнаты
//             await BroadcastToRoom(playerId, roomId, new
//             {
//                 type = "player_left",
//                 playerId = playerId.ToString(),
//                 roomId = roomId,
//                 participants = participants?.Count ?? 0
//             },mediator);
//         }
//         catch (Exception ex)
//         {
//             await SendToPlayer(playerId, new { type = "error", message = ex.Message },mediator);
//         }
//     }
//
//
//
//    
//
//     private static async Task LeaveAllRooms(Guid playerId, IConnectionManager connectionManager,   IMediator mediator)
//     {
//         var userRooms = await connectionManager.GetUserRooms(playerId);
//         foreach (var roomId in userRooms)
//         {
//             await connectionManager.RemoveUserFromRoom(playerId, roomId);
//          
//             var command = new LeaveRoomCommand(roomId, playerId);
//             await mediator.Send(command);
//
//             
//             if (_roomParticipants.TryGetValue(roomId, out var participants))
//             {
//                 
//                 participants.Remove(playerId);
//                 if (participants.Count == 0)
//                 {
//                     _roomParticipants.TryRemove(roomId, out _);
//                     Console.WriteLine($"Room {roomId} is now empty and removed");
//                 }
//                 else
//                 {
//                     // Уведомляем оставшихся участников
//                     await BroadcastToRoom(playerId, roomId, new 
//                     { 
//                         type = "player_disconnected", 
//                         playerId = playerId.ToString(),
//                         participants = participants.Count
//                     },mediator);
//                 }
//             }
//         }
//         
//         Console.WriteLine($"Player {playerId} left all rooms");
//     }
//
//     private static async Task BroadcastToAllRooms(Guid senderId, object message,   IMediator mediator)
//     {
//         var userRooms = await new ConnectionManager().GetUserRooms(senderId);
//         foreach (var roomId in userRooms)
//         {
//             await BroadcastToRoom(senderId, roomId, message,mediator);
//         }
//     }
//
//     private static async Task SendToPlayer(Guid playerId, object message,   IMediator mediator)
//     {
//         if (_sockets.TryGetValue(playerId, out var webSocket) && webSocket.State == WebSocketState.Open)
//         {
//             try
//             {
//                 await SendMessage(webSocket, message);
//             }
//             catch (WebSocketException ex)
//             {
//                 Console.WriteLine($"WebSocket exception when sending to {playerId}: {ex.Message}");
//                 await HandleDisconnection(playerId, new ConnectionManager(), "Send failed", mediator);
//             }
//         }
//     }
//
//     private static async Task BroadcastToRoom(Guid senderId, Guid roomId, object message, IMediator mediator)
//     {
//         if (_roomParticipants.TryGetValue(roomId, out var participants))
//         {
//             var tasks = participants
//                 .Where(participantId => participantId != senderId)
//                 .Select(participantId => SendToPlayer(participantId, message, mediator))
//                 .ToList();
//
//             await Task.WhenAll(tasks);
//         }
//     }
//
//     private static async Task SendMessage(WebSocket webSocket, object message)
//     {
//         var json = JsonSerializer.Serialize(message);
//         var bytes = Encoding.UTF8.GetBytes(json);
//         await webSocket.SendAsync(new ArraySegment<byte>(bytes), 
//             WebSocketMessageType.Text, true, CancellationToken.None);
//     }
//
//     private static Guid GetUserIdFromContext(HttpContext context)
//     {
//         var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
//                           ?? context.User.FindFirst("sub")?.Value
//                           ?? context.User.FindFirst("userId")?.Value;
//
//         if (string.IsNullOrEmpty(userIdClaim))
//         {
//             var testUserId = Guid.NewGuid();
//             Console.WriteLine($"Generated test user ID: {testUserId}");
//             return testUserId;
//         }
//
//         return Guid.Parse(userIdClaim);
//     }
// }
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ByteBattles.Microservices.CodeBattleServer.Application.Commands;
using ByteBattles.Microservices.CodeBattleServer.Application.Queries;
using ByteBattles.Microservices.CodeBattleServer.Domain.Enums;
using ByteBattlesServer.Domain.Results;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.API;

public static class BattleEndpoints
{
    private static readonly ConcurrentDictionary<Guid, WebSocket> _sockets = new();
    private static readonly ConcurrentDictionary<Guid, List<Guid>> _roomParticipants = new();
    private static readonly ConcurrentDictionary<Guid, HashSet<Guid>> _readyPlayers = new();
    private static readonly ConcurrentDictionary<Guid, DateTime> _lastActivity = new();
    private static readonly ConcurrentDictionary<Guid, Timer> _roomTimers = new();
    private static readonly Timer _cleanupTimer;

    static BattleEndpoints()
    {
        _cleanupTimer = new Timer(CleanupInactiveConnections, null, 
            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    public static void MapBattleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/battle")
            .WithTags("Battle");

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

        group.MapGet("/rooms", async (IMediator mediator, HttpContext http) =>
        {
            try
            {
                var activeRooms = new List<object>();
                
                foreach (var room in _roomParticipants)
                {
                    var roomQuery = await mediator.Send(new GetRoomQuery(room.Key));
                    var roomStatus = roomQuery?.Status ?? RoomStatus.Waiting;
                    
                    activeRooms.Add(new { 
                        RoomId = room.Key, 
                        ParticipantCount = room.Value.Count,
                        ReadyCount = _readyPlayers.ContainsKey(room.Key) ? _readyPlayers[room.Key].Count : 0,
                        Status = roomStatus.ToString(),
                        CanStart = room.Value.Count >= 2 && roomStatus == RoomStatus.Active,
                        Participants = room.Value
                    });
                }
                
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

        // Новый endpoint для получения статуса комнаты
        group.MapGet("/rooms/{roomId:guid}/status", async (Guid roomId, IMediator mediator) =>
        {
            try
            {
                var roomQuery = await mediator.Send(new GetRoomQuery(roomId));
                if (roomQuery == null)
                    return Results.NotFound(new { message = "Room not found" });

                var participantCount = _roomParticipants.ContainsKey(roomId) ? _roomParticipants[roomId].Count : 0;
                var readyCount = _readyPlayers.ContainsKey(roomId) ? _readyPlayers[roomId].Count : 0;
                var canStart = participantCount >= 2 && roomQuery.Status == RoomStatus.Active;

                return Results.Ok(new
                {
                    RoomId = roomId,
                    Status = roomQuery.Status.ToString(),
                    ParticipantCount = participantCount,
                    ReadyCount = readyCount,
                    CanStart = canStart,
                    IsActive = roomQuery.Status == RoomStatus.Active,
                    Message = canStart ? "Комната готова к началу игры" : "Недостаточно игроков для начала"
                });
            }
            catch (Exception ex)
            {
                return Results.Problem($"An error occurred: {ex.Message}");
            }
        })
        .WithName("GetRoomStatus")
        .WithSummary("Получение статуса комнаты")
        .WithDescription("Возвращает текущий статус комнаты и информацию о готовности игроков")
        .Produces(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound);
    }

    private static async Task HandleWebSocketConnection(
        WebSocket webSocket, 
        IMediator mediator, 
        IConnectionManager connectionManager, 
        HttpContext httpContext)
    {
        var playerId = GetUserIdFromContext(httpContext);
        _sockets[playerId] = webSocket;
        _lastActivity[playerId] = DateTime.UtcNow;
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
                case "PlayerReady":
                    await SetPlayerReady(playerId,
                        json.RootElement.GetProperty("roomId").GetGuid(),
                        json.RootElement.GetProperty("isReady").GetBoolean(),
                        mediator,
                        connectionManager);
                    break;
                case "pong":
                    _lastActivity[playerId] = DateTime.UtcNow;
                    break;
                default:
                    await SendToPlayer(playerId, new { type = "error", message = $"Unknown message type: {type}" }, mediator);
                    break;
            }
        }
        catch (JsonException ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = $"Invalid JSON format: {ex.Message}" }, mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message }, mediator);
        }
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
                }, mediator);
                return;
            }

            var command = new CreateRoomCommand(roomName, playerId, languageId, difficulty);
            var result = await mediator.Send(command);

            await connectionManager.AddUserToRoom(playerId, result.Id);
            
            _roomParticipants.AddOrUpdate(result.Id,
                new List<Guid> { playerId },
                (_, participants) =>
                {
                    participants.Add(playerId);
                    return participants;
                });

            // Инициализируем набор готовых игроков для комнаты
            _readyPlayers[result.Id] = new HashSet<Guid>();

            await SendToPlayer(playerId, new
            {
                type = "room_created",
                roomId = result.Id,
                roomName = roomName,
                difficulty = difficulty.ToString(),
                languageId = languageId,
                status = "waiting",
                message = $"Комната '{roomName}' создана. Ожидаем второго игрока..."
            }, mediator);

            // Отправляем начальный статус комнаты
            await SendRoomStatus(result.Id, mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message }, mediator);
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
        // Проверяем существование комнаты через медиатор
        var roomQuery = await mediator.Send(new GetRoomQuery(roomId));
        if (roomQuery == null)
        {
            await SendToPlayer(playerId, new { 
                type = "error", 
                message = $"Room {roomId} not found" 
            }, mediator);
            return;
        }

        var command = new JoinRoomCommand(roomId, playerId);
        var result = await mediator.Send(command);
        
        await connectionManager.AddUserToRoom(playerId, roomId);
        
        _roomParticipants.AddOrUpdate(roomId,
            new List<Guid> { playerId },
            (_, participants) =>
            {
                if (!participants.Contains(playerId))
                    participants.Add(playerId);
                return participants;
            });

        // Инициализируем набор готовых игроков если его нет
        _readyPlayers.TryAdd(roomId, new HashSet<Guid>());

        await SendToPlayer(playerId, new
        {
            type = "joined_room",
            roomId = roomId,
            roomName = roomQuery.Name,
            status = roomQuery.Status.ToString(),
            message = $"Вы присоединились к комнате {roomQuery.Name}",
            participants = _roomParticipants[roomId].Count,
            // ИСПРАВЛЕНО: Проверяем готовность на основе данных в памяти
            canStart = _roomParticipants[roomId].Count >= 2
        }, mediator);

        // Уведомляем других участников комнаты
        await BroadcastToRoom(playerId, roomId, new
        {
            type = "player_joined",
            playerId = playerId.ToString(),
            roomId = roomId,
            participants = _roomParticipants[roomId].Count,
            roomStatus = roomQuery.Status.ToString()
        }, mediator);

        // Отправляем обновленный статус комнаты всем участникам
        await SendRoomStatus(roomId, mediator);

        // ИСПРАВЛЕНО: Проверяем готовность на основе данных в памяти
        if (_roomParticipants[roomId].Count >= 2)
        {
            await BroadcastToRoom(Guid.Empty, roomId, new
            {
                type = "game_can_start",
                roomId = roomId,
                message = "Комната заполнена! Подтвердите готовность в течение 10 секунд.",
                countdown = 10
            }, mediator);

            // Запускаем таймер ожидания готовности
            StartReadinessTimer(roomId, mediator);
        }
    }
    catch (Exception ex)
    {
        await SendToPlayer(playerId, new { type = "error", message = ex.Message }, mediator);
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
private static async Task SetPlayerReady(
    Guid playerId, 
    Guid roomId, 
    bool isReady,
    IMediator mediator,
    IConnectionManager connectionManager)
{
    try
    {
        if (!_roomParticipants.ContainsKey(roomId))
        {
            await SendToPlayer(playerId, new { 
                type = "error", 
                message = "Room not found" 
            }, mediator);
            return;
        }

        if (!_roomParticipants[roomId].Contains(playerId))
        {
            await SendToPlayer(playerId, new { 
                type = "error", 
                message = "You are not in this room" 
            }, mediator);
            return;
        }

        // ПРОВЕРКА ИСПРАВЛЕНА: Используем данные из памяти, а не из БД
        var participantCount = _roomParticipants[roomId].Count;
        var canStart = participantCount >= 2;
        
        if (!canStart)
        {
            await SendToPlayer(playerId, new { 
                type = "error", 
                message = "Not enough players to start the game" 
            }, mediator);
            return;
        }

        if (isReady)
        {
            _readyPlayers[roomId].Add(playerId);
            await SendToPlayer(playerId, new
            {
                type = "player_ready_set",
                roomId = roomId,
                isReady = true,
                message = "Вы подтвердили готовность"
            }, mediator);
        }
        else
        {
            _readyPlayers[roomId].Remove(playerId);
            await SendToPlayer(playerId, new
            {
                type = "player_ready_set",
                roomId = roomId,
                isReady = false,
                message = "Вы отменили готовность"
            }, mediator);
        }

        // Уведомляем всех о изменении статуса готовности
        await BroadcastToRoom(playerId, roomId, new
        {
            type = "player_ready_changed",
            playerId = playerId.ToString(),
            roomId = roomId,
            isReady = isReady,
            readyCount = _readyPlayers[roomId].Count,
            totalPlayers = _roomParticipants[roomId].Count
        }, mediator);

        // Проверяем, все ли игроки готовы
        if (_readyPlayers[roomId].Count == _roomParticipants[roomId].Count && 
            _roomParticipants[roomId].Count >= 2)
        {
            await StartGame(roomId, mediator);
        }
        
        // Запускаем/перезапускаем таймер ожидания готовности
        if (_readyPlayers[roomId].Count > 0)
        {
            StartReadinessTimer(roomId, mediator);
        }
    }
    catch (Exception ex)
    {
        await SendToPlayer(playerId, new { type = "error", message = ex.Message }, mediator);
    }
}
    private static void CleanupPlayer(Guid playerId, IConnectionManager connectionManager)
     {
         _sockets.TryRemove(playerId, out _);
         _lastActivity.TryRemove(playerId, out _);
         connectionManager.RemoveConnection(playerId, "cleanup").Wait();
         
         Console.WriteLine($"Player {playerId} cleaned up. Remaining connections: {_sockets.Count}");
     }

   
    private static void StartReadinessTimer(Guid roomId, IMediator mediator)
    {
        // Останавливаем предыдущий таймер если он есть
        if (_roomTimers.TryRemove(roomId, out var oldTimer))
        {
            oldTimer?.Dispose();
        }

        var timer = new Timer(async _ =>
        {
            await CheckReadinessTimeout(roomId, mediator);
        }, null, TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);

        _roomTimers[roomId] = timer;

        Console.WriteLine($"Started readiness timer for room {roomId}");
    }

    private static async Task CheckReadinessTimeout(Guid roomId, IMediator mediator)
    {
        try
        {
            if (_roomParticipants.TryGetValue(roomId, out var participants) &&
                _readyPlayers.TryGetValue(roomId, out var readyPlayers))
            {
                if (readyPlayers.Count == participants.Count && participants.Count >= 2)
                {
                    // Все игроки готовы - начинаем игру
                    await StartGame(roomId, mediator);
                }
                else
                {
                    // Не все готовы - отменяем
                    await BroadcastToRoom(Guid.Empty, roomId, new
                    {
                        type = "readiness_timeout",
                        roomId = roomId,
                        message = "Время ожидания истекло. Не все игроки подтвердили готовность.",
                        readyCount = readyPlayers.Count,
                        totalPlayers = participants.Count
                    }, mediator);

                    // Сбрасываем статусы готовности
                    readyPlayers.Clear();
                }

                // Очищаем таймер
                _roomTimers.TryRemove(roomId, out _);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in readiness timeout for room {roomId}: {ex.Message}");
        }
    }

    private static async Task StartGame(Guid roomId, IMediator mediator)
    {
        try
        {
            // Здесь должна быть логика начала игры
            // Например, выбор задачи, установка таймера и т.д.

            await BroadcastToRoom(Guid.Empty, roomId, new
            {
                type = "game_started",
                roomId = roomId,
                message = "Игра началась! У вас есть 30 минут на решение задачи.",
                startTime = DateTime.UtcNow,
                duration = 1800 // 30 минут в секундах
            }, mediator);

            // Очищаем статусы готовности
            if (_readyPlayers.TryGetValue(roomId, out var readyPlayers))
            {
                readyPlayers.Clear();
            }

            // Останавливаем таймер готовности
            if (_roomTimers.TryRemove(roomId, out var timer))
            {
                timer?.Dispose();
            }

            Console.WriteLine($"Game started in room {roomId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting game in room {roomId}: {ex.Message}");
        }
    }

    private static async Task SendRoomStatus(Guid roomId, IMediator mediator)
    {
        try
        {
            var roomQuery = await mediator.Send(new GetRoomQuery(roomId));
            if (roomQuery == null) return;

            var participantCount = _roomParticipants.ContainsKey(roomId) ? _roomParticipants[roomId].Count : 0;
            var readyCount = _readyPlayers.ContainsKey(roomId) ? _readyPlayers[roomId].Count : 0;
            var canStart = participantCount >= 2 && roomQuery.Status == RoomStatus.Active;

            var statusMessage = new
            {
                type = "room_status",
                roomId = roomId,
                status = roomQuery.Status.ToString(),
                participantCount = participantCount,
                readyCount = readyCount,
                canStart = canStart,
                isActive = roomQuery.Status == RoomStatus.Active
            };

            await BroadcastToRoom(Guid.Empty, roomId, statusMessage, mediator);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending room status for {roomId}: {ex.Message}");
        }
    }

    // Остальные методы (LeaveRoom, SubmitCode, HandleDisconnection, CleanupPlayer, etc.)
    // остаются аналогичными предыдущей реализации, но с добавлением mediator параметра

    private static async Task LeaveRoom(
        Guid playerId, 
        Guid roomId, 
        IConnectionManager connectionManager,
        IMediator mediator)
    {
        try
        {
            await connectionManager.RemoveUserFromRoom(playerId, roomId);
            
            // Удаляем из участников комнаты
            if (_roomParticipants.TryGetValue(roomId, out var participants))
            {
                participants.Remove(playerId);
                
                // Удаляем из готовых игроков
                if (_readyPlayers.TryGetValue(roomId, out var readyPlayers))
                {
                    readyPlayers.Remove(playerId);
                }

                if (participants.Count == 0)
                {
                    _roomParticipants.TryRemove(roomId, out _);
                    _readyPlayers.TryRemove(roomId, out _);
                    
                    // Останавливаем таймер если комната пустая
                    if (_roomTimers.TryRemove(roomId, out var timer))
                    {
                        timer?.Dispose();
                    }
                }
            }

            var command = new LeaveRoomCommand(roomId, playerId);
            await mediator.Send(command);

            await SendToPlayer(playerId, new
            {
                type = "left_room",
                roomId = roomId,
                message = $"Вы покинули комнату {roomId}"
            }, mediator);

            // Уведомляем других участников комнаты и отправляем обновленный статус
            await BroadcastToRoom(playerId, roomId, new
            {
                type = "player_left",
                playerId = playerId.ToString(),
                roomId = roomId,
                participants = participants?.Count ?? 0
            }, mediator);

            await SendRoomStatus(roomId, mediator);
        }
        catch (Exception ex)
        {
            await SendToPlayer(playerId, new { type = "error", message = ex.Message }, mediator);
        }
    }

    // ... остальные методы (SubmitCode, HandleDisconnection, CleanupPlayer, CleanupInactiveConnections, etc.)
    // должны быть адаптированы аналогично

    private static async Task SendToPlayer(Guid playerId, object message, IMediator mediator)
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
}