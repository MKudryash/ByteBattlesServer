namespace ByteBattles.Microservices.CodeBattleServer.Application.DTOs;

public record TestResultDto
{
    public Guid Id { get; init; }
    public string Status { get; init; }
    public string? Input { get; init; }
    public string? ExpectedOutput { get; init; }
    public string? ActualOutput { get; init; }
    public string? ErrorMessage { get; init; }
    public TimeSpan ExecutionTime { get; init; }
    public int MemoryUsed { get; init; }
}