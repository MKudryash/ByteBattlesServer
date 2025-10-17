using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;


public class UserProfile : Entity
{
    public Guid UserId { get; private set; }
    public string UserName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }
    public string? Country { get; private set; }
    public string? GitHubUrl { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public UserLevel Level { get; private set; }
    public UserStats Stats { get; private set; }
    public UserSettings Settings { get; private set; }
    public bool IsPublic { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; } // Оставляем приватный сеттер

    private readonly List<UserAchievement> _achievements = new();
    public IReadOnlyCollection<UserAchievement> Achievements => _achievements.AsReadOnly();

    private readonly List<BattleResult> _battleHistory = new();
    public IReadOnlyCollection<BattleResult> BattleHistory => _battleHistory.AsReadOnly();

    private UserProfile() { }

    public UserProfile(Guid userId, string userName)
    {
        UserId = userId;
        UserName = userName;
        Level = UserLevel.Beginner;
        Stats = new UserStats();
        Settings = new UserSettings();
        IsPublic = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string userName, string? bio, string? country, 
        string? githubUrl, string? linkedInUrl, bool isPublic)
    {
        UserName = userName;
        Bio = bio;
        Country = country;
        GitHubUrl = githubUrl;
        LinkedInUrl = linkedInUrl;
        IsPublic = isPublic;
        UpdateTimestamps();
    }

    public void UpdateAvatar(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
        UpdateTimestamps();
    }

    public void AddAchievement(Achievement achievement)
    {
        if (!_achievements.Any(a => a.AchievementId == achievement.Id))
        {
            var userAchievement = new UserAchievement(Id, achievement.Id);
            _achievements.Add(userAchievement);
            UpdateTimestamps();
        }
    }

    public void AddBattleResult(BattleResult battleResult)
    {
        _battleHistory.Add(battleResult);
        Stats.UpdateStats(battleResult);
        UpdateLevel();
        UpdateTimestamps();
    }

    private void UpdateLevel()
    {
        Level = UserLevelCalculator.CalculateLevel(Stats.TotalExperience);
    }

    // Внутренний метод для обновления временных меток
    private void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    // Метод для EF Core чтобы обновлять UpdatedAt при сохранении
    public void SetUpdatedAt(DateTime updatedAt)
    {
        UpdatedAt = updatedAt;
    }
}