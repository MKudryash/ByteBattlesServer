using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;


public class UserProfile : Entity
{
    public Guid UserId { get; private set; }
    public string UserName { get; set; }
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public UserLevel Level { get; private set; }
    public UserStats Stats { get; private set; }
    public UserSettings Settings { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; set; } 

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
        if (!string.IsNullOrWhiteSpace(userName))
            UserName =userName.Trim();
        
        if (bio != null)
            Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        
        if (country != null)
            Country = string.IsNullOrWhiteSpace(country) ? null : country.Trim();
        
        if (!string.IsNullOrWhiteSpace(githubUrl))
            GitHubUrl = string.IsNullOrWhiteSpace(githubUrl) ? null : githubUrl.Trim();
        
        if (!string.IsNullOrWhiteSpace(linkedInUrl))
            LinkedInUrl = string.IsNullOrWhiteSpace(linkedInUrl) ? null : linkedInUrl.Trim();

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
    public void UpdateSettings(
        string? preferredLanguage = null,
        string? theme = null,
        string? codeEditorTheme = null,
        bool? achievementNotifications = null,
        bool? battleInvitations = null,
        bool? emailNotifications = null)
    {
        if (Settings == null)
        {
            Settings = new UserSettings();
        }

        Settings.Update(
            preferredLanguage,
            theme,
            codeEditorTheme,
            achievementNotifications,
            battleInvitations,
            emailNotifications
        );

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