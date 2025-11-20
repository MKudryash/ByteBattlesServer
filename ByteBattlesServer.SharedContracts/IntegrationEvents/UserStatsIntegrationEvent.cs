namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class UserStatsIntegrationEvent
{
    public Guid UserId { get; set; }
    public bool IsSuccessful { get; set; }
    public TaskDifficulty Difficulty { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public Guid TaskId { get; set; }
    public string ProblemTitle { get; set; } = "";
    public string Language { get; set; } = "";
}