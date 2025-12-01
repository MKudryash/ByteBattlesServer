using ByteBattlesServer.Domain.enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;



public interface IUserProfileRepository
{
    
    // Базовые методы
    Task<Entities.UserProfile> GetByIdAsync(Guid id);
    Task<Entities.UserProfile> GetByIdTeacherAsync(Guid id);
    Task<Entities.UserProfile> GetByUserIdAsync(Guid userId);
    Task<Entities.UserProfile> GetByUserIdTeacherAsync(Guid userId);
    Task<List<Entities.UserProfile>> GetLeaderboardAsync(int topCount);
    Task<List<Entities.UserProfile>> SearchAsync(string searchTerm, int page, int pageSize);
    Task AddAsync(Entities.UserProfile userProfile);
    void Update(Entities.UserProfile userProfile);
    
    Task<Domain.Entities.UserProfile> GetTeacherProfileAsync(Guid userId);
    
    // Методы для активности
    Task AddRecentActivityAsync(Entities.RecentActivity activity);
    Task AddRecentProblemAsync(Entities.RecentProblem problem);
    Task<List<Entities.RecentActivity>> GetRecentActivitiesAsync(Guid userProfileId, int count = 50);
    Task<List<Entities.RecentProblem>> GetRecentProblemsAsync(Guid userProfileId, int count = 20);
   
    Task<bool> HasUserSolvedProblemAsync(Guid userProfileId, Guid problemId);
    Task<(int TotalActivities, int TodayActivities)> GetActivityStatsAsync(Guid userProfileId);

    // Новые методы для ролей

    Task<Entities.UserProfile> GetStudentProfileAsync(Guid userId);
    Task UpdateTeacherStatsAsync(Guid userId, int? createdTasks = null, int? activeStudents = null, 
        double? averageRating = null, int? totalSubmissions = null);
    Task<bool> ExistsByEmailAsync(string email);
    Task<Entities.UserProfile> GetByEmailAsync(string email);
    
}