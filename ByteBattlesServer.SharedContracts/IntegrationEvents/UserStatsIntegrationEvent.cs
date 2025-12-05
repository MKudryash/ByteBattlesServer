using ByteBattlesServer.Domain.enums;

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
    public ActivityType  ActivityType { get; set; }
    public Guid BattleId { get; set; }
    public string BattleOponent { get; set; } = "";
}

public enum ActivityType
{
    Battle = 0,
    ProblemSolved = 1,
    Achievement = 2
}