namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class UserStatsIntegrationEvent
{
    public Guid UserId { get; set; }
    public bool isSuccessful { get; set; }
    public TaskDifficulty difficulty { get; set; }
    public TimeSpan executionTime { get; set; }
    public Guid taskId { get; set; }
}
public enum TaskDifficulty
{
    Easy =1,
    Medium =2,
    Hard=3
}
public class UserStatsUpdateIntegrationEvent
{
    public Guid UserId{ get; set; }
    public int TotalProblemsSolved { get; set; }
    public int TotalBattles { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int CurrentStreak { get; set; }
    public int MaxStreak { get; set; }
    public int TotalExperience { get; set; }
    public double WinRate { get; set; }
    public int ExperienceToNextLevel { get; set; }
}