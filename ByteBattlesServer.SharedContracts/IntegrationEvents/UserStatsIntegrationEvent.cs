namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class UserStatsIntegrationEvent
{
    public Guid UserId { get; set; }
    public bool isSuccessful { get; set; }
    public TaskDifficulty difficulty { get; set; }
    public TimeSpan executionTime { get; set; }
    public Guid taskId { get; set; }
}