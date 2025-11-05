namespace ByteBattlesServer.Microservices.SolutionService.Application.DTOs;

public record SolutionAttemptDto
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public DateTime AttemptedAt { get; init; }
    public string Status { get; init; }
    public TimeSpan? ExecutionTime { get; init; }
    public int? MemoryUsed { get; init; }
}