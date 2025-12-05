namespace ByteBattles.Microservices.CodeBattleServer.API;

public interface IConnectionManager
{
    Task AddConnection(Guid userId, string connectionId);
    Task RemoveConnection(Guid userId, string connectionId);
    Task AddUserToRoom(Guid userId, Guid roomId);
    Task RemoveUserFromRoom(Guid userId, Guid roomId);
    Task<List<Guid>> GetUserRooms(Guid userId);
}