using System.Collections.Concurrent;

namespace ByteBattles.Microservices.CodeBattleServer.API;

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