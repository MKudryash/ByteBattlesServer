namespace ByteBattles.Microservices.CodeBattleServer.Domain.Entities;

public class CodeSubmission : Entity
{
    public Guid RoomId { get; private set; }
    public Guid UserId { get; private set; }
    public string ProblemId { get; private set; }
    public string Code { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public SubmissionResult? Result { get; private set; }

    private CodeSubmission() { }

    public CodeSubmission(Guid roomId, Guid userId, string problemId, string code)
    {
        Id = Guid.NewGuid();
        RoomId = roomId;
        UserId = userId;
        ProblemId = problemId;
        Code = code;
        SubmittedAt = DateTime.UtcNow;
    }

    public void SetResult(SubmissionResult result)
    {
        Result = result;
    }
}