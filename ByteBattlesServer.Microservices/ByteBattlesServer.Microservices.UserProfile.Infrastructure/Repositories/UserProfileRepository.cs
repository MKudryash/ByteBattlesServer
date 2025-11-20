using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly UserProfileDbContext _context;

    public UserProfileRepository(UserProfileDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.UserProfile> GetByIdAsync(Guid id)
    {
        return await _context.UserProfiles
            .Include(up => up.Achievements)
                .ThenInclude(ua => ua.Achievement)
            .Include(up => up.BattleHistory)
            .Include(up => up.RecentActivities)
            .Include(up => up.RecentProblems)
            .FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<Domain.Entities.UserProfile> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(up => up.Achievements)
                .ThenInclude(ua => ua.Achievement)
            .Include(up => up.BattleHistory)
            .Include(up => up.RecentActivities)
            .Include(up => up.RecentProblems)
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<List<Domain.Entities.UserProfile>> GetLeaderboardAsync(int topCount)
    {
        return await _context.UserProfiles
            .Where(up => up.IsPublic)
            .OrderByDescending(up => up.Stats.TotalExperience)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.UserProfile>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        return await _context.UserProfiles
            .Where(up => up.IsPublic && 
                (up.UserName.Contains(searchTerm) || 
                 (up.Bio != null && up.Bio.Contains(searchTerm))))
            .OrderByDescending(up => up.Stats.TotalExperience)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(Domain.Entities.UserProfile userProfile)
    {
        await _context.UserProfiles.AddAsync(userProfile);
    }

    public void Update(Domain.Entities.UserProfile userProfile)
    {
        _context.UserProfiles.Update(userProfile);
    }
    

    public async Task AddRecentActivityAsync(Domain.Entities.RecentActivity activity)
    {
        await _context.RecentActivities.AddAsync(activity);
    }

    public async Task AddRecentProblemAsync(Domain.Entities.RecentProblem problem)
    {
        await _context.RecentProblems.AddAsync(problem);
    }

    public async Task<List<Domain.Entities.RecentActivity>> GetRecentActivitiesAsync(Guid userProfileId, int count = 50)
    {
        return await _context.RecentActivities
            .Where(ra => ra.UserProfileId == userProfileId)
            .OrderByDescending(ra => ra.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.RecentProblem>> GetRecentProblemsAsync(Guid userProfileId, int count = 20)
    {
        return await _context.RecentProblems
            .Where(rp => rp.UserProfileId == userProfileId)
            .OrderByDescending(rp => rp.SolvedAt)
            .Take(count)
            .ToListAsync();
    }
    

    // Метод для получения пользователя с недавней активностью (оптимизированная загрузка)
    public async Task<Domain.Entities.UserProfile> GetUserWithRecentActivityAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(up => up.RecentActivities.OrderByDescending(ra => ra.Timestamp).Take(10))
            .Include(up => up.RecentProblems.OrderByDescending(rp => rp.SolvedAt).Take(10))
            .Include(up => up.Achievements.OrderByDescending(a => a.AchievedAt).Take(5))
                .ThenInclude(ua => ua.Achievement)
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    // Метод для проверки существования проблемы в истории пользователя
    public async Task<bool> HasUserSolvedProblemAsync(Guid userProfileId, Guid problemId)
    {
        return await _context.RecentProblems
            .AnyAsync(rp => rp.UserProfileId == userProfileId && rp.ProblemId == problemId);
    }

    // Метод для получения статистики по активностям
    public async Task<(int TotalActivities, int TodayActivities)> GetActivityStatsAsync(Guid userProfileId)
    {
        var today = DateTime.UtcNow.Date;
        
        var totalActivities = await _context.RecentActivities
            .CountAsync(ra => ra.UserProfileId == userProfileId);
            
        var todayActivities = await _context.RecentActivities
            .CountAsync(ra => ra.UserProfileId == userProfileId && ra.Timestamp.Date == today);

        return (totalActivities, todayActivities);
    }
}