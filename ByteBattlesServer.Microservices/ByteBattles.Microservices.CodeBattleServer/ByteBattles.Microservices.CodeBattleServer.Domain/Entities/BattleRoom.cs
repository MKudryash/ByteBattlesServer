using ByteBattles.Microservices.CodeBattleServer.Domain.Enums;

namespace ByteBattles.Microservices.CodeBattleServer.Domain.Entities;


public class BattleRoom : Entity
{
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public RoomStatus Status { get; private set; }
    
    private readonly List<RoomParticipant> _participants = new();
    public IReadOnlyCollection<RoomParticipant> Participants => _participants.AsReadOnly();
    
    private readonly List<CodeSubmission> _submissions = new();
    public IReadOnlyCollection<CodeSubmission> Submissions => _submissions.AsReadOnly();

    private BattleRoom() { }

    public BattleRoom(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        CreatedAt = DateTime.UtcNow;
        Status = RoomStatus.Waiting;
    }

    public void AddParticipant(Guid userId)
    {
        if (_participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("User already in room");

        _participants.Add(new RoomParticipant(Id, userId));
        
        if (_participants.Count >= 2)
            Status = RoomStatus.Active;
    }

    public void RemoveParticipant(Guid userId)
    {
        var participant = _participants.FirstOrDefault(p => p.UserId == userId);
        if (participant != null)
            _participants.Remove(participant);
        
        if (_participants.Count < 2)
            Status = RoomStatus.Waiting;
    }

    public void AddSubmission(CodeSubmission submission)
    {
        _submissions.Add(submission);
    }
}