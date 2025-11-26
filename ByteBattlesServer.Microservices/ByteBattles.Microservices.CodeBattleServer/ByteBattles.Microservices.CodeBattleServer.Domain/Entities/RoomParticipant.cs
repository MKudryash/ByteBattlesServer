namespace ByteBattles.Microservices.CodeBattleServer.Domain.Entities;

public class RoomParticipant
{
    public Guid RoomId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public RoomParticipant(Guid roomId, Guid userId)
    {
        RoomId = roomId;
        UserId = userId;
        JoinedAt = DateTime.UtcNow;
    }
}