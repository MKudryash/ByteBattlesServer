namespace ByteBattles.Microservices.CodeBattleServer.Domain.Entities;

public record SubmissionResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; }
    public int TestsPassed { get; init; }
    public int TotalTests { get; init; }
    public TimeSpan ExecutionTime { get; init; }
}