using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

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
    public UserStats Stats { get; set; }
    public UserSettings Settings { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; set; } 

    private readonly List<UserAchievement> _achievements = new();
    public IReadOnlyCollection<UserAchievement> Achievements => _achievements.AsReadOnly();

    private readonly List<BattleResult> _battleHistory = new();
    public IReadOnlyCollection<BattleResult> BattleHistory => _battleHistory.AsReadOnly();
    
    private readonly List<RecentActivity> _recentActivities = new();
    public IReadOnlyCollection<RecentActivity> RecentActivities => _recentActivities.AsReadOnly();

    private readonly List<RecentProblem> _recentProblems = new();
    public IReadOnlyCollection<RecentProblem> RecentProblems => _recentProblems.AsReadOnly();

    private UserProfile() { }

    public UserProfile(Guid userId, string userName, bool isPublic)
    {
        UserId = userId;
        UserName = userName;
        Level = UserLevel.Beginner;
        Stats = new UserStats();
        Settings = new UserSettings();
        IsPublic = isPublic;
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
    public void AddRecentActivity(ActivityType type, string title, string description, int experienceGained = 0)
    {
        var activity = new RecentActivity(Id, type, title, description, experienceGained);
        _recentActivities.Add(activity);
        
        if (_recentActivities.Count > 50)
        {
            _recentActivities.RemoveAt(0);
        }
        
        UpdateTimestamps();
    }

    public void AddRecentProblem(Guid problemId, string title, TaskDifficulty difficulty, string language)
    {
        var recentProblem = new RecentProblem(Id, problemId, title, difficulty, language);
        _recentProblems.Add(recentProblem);
        
        if (_recentProblems.Count > 20)
        {
            _recentProblems.RemoveAt(0);
        }
        
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

    public void UpdateProblemStats(bool isSuccessful, TaskDifficulty difficulty, 
        TimeSpan executionTime, Guid taskId, string problemTitle, string language)
    {
        if (Stats == null)
        {
            Stats = new UserStats();
        }
        
        Stats.UpdateProblemStats(isSuccessful, difficulty, executionTime, taskId);
        
        if (isSuccessful)
        {
            AddRecentProblem(taskId, problemTitle, difficulty, language);
            
            var expGained = difficulty switch
            {
                TaskDifficulty.Easy => 10,
                TaskDifficulty.Medium => 25,
                TaskDifficulty.Hard => 50,
                _ => 0
            };
            
            AddRecentActivity(ActivityType.ProblemSolved, 
                $"Решена задача: {problemTitle}", 
                $"Сложность: {difficulty}, Язык: {language}",
                expGained);
        }
        
        UpdateLevel();
        UpdateTimestamps();
    }




    public void AddBattleResult(BattleResult battleResult)
    {
        _battleHistory.Add(battleResult);
        Stats.UpdateStats(battleResult);
        
        // Добавляем активность о битве
        var activityDescription = $"Результат: {battleResult.Result}, Опыт: {battleResult.ExperienceGained}";
        AddRecentActivity(ActivityType.Battle, 
            $"Завершена битва", 
            activityDescription,
            battleResult.ExperienceGained);
            
        UpdateLevel();
        UpdateTimestamps();
    }

    public void AddAchievement(Achievement achievement)
    {
        if (!_achievements.Any(a => a.AchievementId == achievement.Id))
        {
            var userAchievement = new UserAchievement(Id, achievement.Id);
            _achievements.Add(userAchievement);
            
            // Добавляем активность о достижении
            AddRecentActivity(ActivityType.Achievement, 
                "Получено достижение", 
                achievement.Description);
                
            UpdateTimestamps();
        }
    }

    private void UpdateLevel()
    {
        Level = UserLevelCalculator.CalculateLevel(Stats.TotalExperience);
    }
    
    private void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetUpdatedAt(DateTime updatedAt)
    {
        UpdatedAt = updatedAt;
    }
}