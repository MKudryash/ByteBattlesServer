using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;


public class UserProfile : Entity
{
    public Guid UserId { get; private set; }
    public string UserName { get; set; }
    public string? Email { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public UserRole Role { get; private set; } // Добавляем поле роли
    
    // Поля для студента
    public UserLevel Level { get; private set; }
    public UserStats Stats { get; set; }
    
    // Поля для учителя
    public TeacherStats TeacherStats { get; set; }
    
    // Общие настройки и коллекции
    public UserSettings Settings { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; set; }

    // Навигационные свойства
    private readonly List<UserAchievement> _achievements = new();
    public IReadOnlyCollection<UserAchievement> Achievements => _achievements.AsReadOnly();

    private readonly List<BattleResult> _battleHistory = new();
    public IReadOnlyCollection<BattleResult> BattleHistory => _battleHistory.AsReadOnly();
    
    private readonly List<RecentActivity> _recentActivities = new();
    public IReadOnlyCollection<RecentActivity> RecentActivities => _recentActivities.AsReadOnly();

    private readonly List<RecentProblem> _recentProblems = new();
    public IReadOnlyCollection<RecentProblem> RecentProblems => _recentProblems.AsReadOnly();

    // Конструкторы
    private UserProfile() { }

    public UserProfile(Guid userId, string userName, string email, UserRole role, bool isPublic)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        Role = role;
        IsPublic = isPublic;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        // Инициализация в зависимости от роли
        if (role == UserRole.student)
        {
            Level = UserLevel.Beginner;
            Stats = new UserStats();
        }
        else if (role == UserRole.teacher)
        {
            TeacherStats = new TeacherStats();
        }
        
        Settings = new UserSettings();
    }


    public void UpdateProfile(string userName, string? bio, 
        string? country, string? githubUrl, string? linkedInUrl, bool isPublic)
    {
        if (!string.IsNullOrWhiteSpace(userName))
            UserName = userName.Trim();
        
        if (bio != null)
            Bio = string.IsNullOrWhiteSpace(bio) ? null : bio.Trim();
        
        if (country != null)
            Country = string.IsNullOrWhiteSpace(country) ? null : country.Trim();
        
        if (githubUrl != null)
            GitHubUrl = string.IsNullOrWhiteSpace(githubUrl) ? null : githubUrl.Trim();
        
        if (linkedInUrl != null)
            LinkedInUrl = string.IsNullOrWhiteSpace(linkedInUrl) ? null : linkedInUrl.Trim();

        IsPublic = isPublic;
        UpdateTimestamps();
    }
    public void UpdateTeacherStats(int? createdTasks = null, int? activeStudents = null, 
        double? averageRating = null, int? totalSubmissions = null)
    {
        if (Role != UserRole.teacher)
            throw new InvalidOperationException("Only teachers can update teacher stats");

        TeacherStats?.UpdateTeacherStats(createdTasks, activeStudents, averageRating, totalSubmissions);
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

    public void UpdateProblemStats(bool isSuccessful, TaskDifficulty difficulty, 
        TimeSpan executionTime, Guid taskId)
    {
       
        
        Stats.UpdateProblemStats(isSuccessful, difficulty, executionTime, taskId);
        
        
        UpdateLevel();
        UpdateTimestamps();
    }

    public void AddTotalSubmissions()
    {
        Stats.IncrementProblemsSolved(0);
    }






    public void UpdateLevel()
    {
        Level = UserLevelCalculator.CalculateLevel(Stats.TotalExperience);
    }
    
    public void UpdateTimestamps()
    {
        UpdatedAt = DateTime.UtcNow;
    }
    
}