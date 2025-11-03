using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface IUserServiceClient
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task UpdateUserStatsAsync(Guid userId, bool isSuccessful, TimeSpan executionTime, string taskDifficulty, Guid taskId);
    Task<List<AchievementDto>> GetUserAchievementsAsync(Guid userId);
}
public class AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Type { get; set; }
    public int RequiredValue { get; set; }
    public int RewardExperience { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedAt { get; set; }
}

public class UserStatsDto
{
    public int TotalProblemsSolved { get; set; }
    public int EasyProblemsSolved { get; set; }
    public int MediumProblemsSolved { get; set; }
    public int HardProblemsSolved { get; set; }
    public int TotalSubmissions { get; set; }
    public int SuccessfulSubmissions { get; set; }
    public double SuccessRate { get; set; }
    public TimeSpan AverageExecutionTime { get; set; }
    public int TotalExperience { get; set; }
    public string CurrentLevel { get; set; }
    public int CurrentStreak { get; set; }
    public int MaxStreak { get; set; }
}