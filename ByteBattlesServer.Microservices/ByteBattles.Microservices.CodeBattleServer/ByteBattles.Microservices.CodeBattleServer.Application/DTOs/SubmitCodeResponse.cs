using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

public class SubmitCodeResponse
{
    public Guid Id { get; init; }
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public LanguageInfo LanguageId { get; init; }
    public TestStatus Status { get; set; }
    public DateTime SubmittedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public TimeSpan? ExecutionTime { get; set; }
    public int? MemoryUsed { get; init; }
    public int PassedTests { get; init; }
    public int TotalTests { get; init; }
    public double SuccessRate { get; init; }
    public List<TestResultDto> TestResults { get; init; } = new();
    public List<SolutionAttemptDto> Attempts { get; init; } = new();
}