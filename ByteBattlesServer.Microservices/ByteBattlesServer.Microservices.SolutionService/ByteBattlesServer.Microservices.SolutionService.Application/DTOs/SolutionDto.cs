namespace ByteBattlesServer.Microservices.SolutionService.Application.DTOs;

public record SolutionDto
{
    public Guid Id { get; init; }
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public Guid LanguageId { get; init; }
    public string Status { get; init; }
    public DateTime SubmittedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public TimeSpan? ExecutionTime { get; init; }
    public int? MemoryUsed { get; init; }
    public int PassedTests { get; init; }
    public int TotalTests { get; init; }
    public double SuccessRate { get; init; }
    public List<TestResultDto> TestResults { get; init; } = new();
    public List<SolutionAttemptDto> Attempts { get; init; } = new();
}

public record SolutionTaskAndUserDto
{
    public Guid UserId { get; init; }
    public Guid TaskId { get; init; }
}