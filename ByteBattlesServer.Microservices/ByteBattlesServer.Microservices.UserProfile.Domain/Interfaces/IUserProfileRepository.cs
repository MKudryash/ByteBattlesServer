namespace ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;

using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;


public interface IUserProfileRepository
{
    Task<UserProfile> GetByIdAsync(Guid id);
    Task<UserProfile> GetByUserIdAsync(Guid userId);
    Task<List<UserProfile>> GetLeaderboardAsync(int topCount);
    Task<List<UserProfile>> SearchAsync(string searchTerm, int page, int pageSize);
    Task AddAsync(UserProfile userProfile);
    void Update(UserProfile userProfile);
    
    // Новые методы
    Task AddRecentActivityAsync(RecentActivity activity);
    Task AddRecentProblemAsync(RecentProblem problem);
    Task<List<RecentActivity>> GetRecentActivitiesAsync(Guid userProfileId, int count = 50);
    Task<List<RecentProblem>> GetRecentProblemsAsync(Guid userProfileId, int count = 20);
    Task<UserProfile> GetUserWithRecentActivityAsync(Guid userId);
    Task<bool> HasUserSolvedProblemAsync(Guid userProfileId, Guid problemId);
    Task<(int TotalActivities, int TodayActivities)> GetActivityStatsAsync(Guid userProfileId);
}