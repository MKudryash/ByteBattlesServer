using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
            .Include(up => up.Stats)
            .Include(up => up.Settings)
            .FirstOrDefaultAsync(up => up.Id == id);
    }  

    public async Task<Domain.Entities.UserProfile> GetByIdTeacherAsync(Guid id)
    {
        return await _context.UserProfiles
            .Include(up => up.RecentActivities)
            .Include(up => up.TeacherStats)
            .Include(up => up.Settings)
            .FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<Domain.Entities.UserProfile> GetByUserIdAsync(Guid? userId)
    {
        return await _context.UserProfiles
            .Include(up => up.Achievements)
                .ThenInclude(ua => ua.Achievement)
            .Include(up => up.BattleHistory)
            .Include(up => up.RecentActivities)
            .Include(up => up.RecentProblems)
            .Include(up => up.Stats)
            .Include(up => up.Settings)
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }   

    public async Task<Domain.Entities.UserProfile> GetByUserIdTeacherAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(up => up.RecentActivities)
            .Include(up => up.TeacherStats)
            .Include(up => up.Settings)
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    
    public async Task<Domain.Entities.UserProfile> GetStudentProfileAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(up => up.Stats)
            .Include(up => up.Settings)
            .Include(up => up.RecentProblems.OrderByDescending(rp => rp.SolvedAt).Take(10))
            .Include(up => up.BattleHistory.OrderByDescending(bh => bh.BattleDate).Take(10))
            .Include(up => up.RecentActivities.OrderByDescending(ra => ra.Timestamp).Take(10))
            .Include(up => up.Achievements.OrderByDescending(a => a.UnlockedAt).Take(5))
                .ThenInclude(ua => ua.Achievement)
            .Where(up => up.UserId == userId && up.Role == UserRole.student)
            .FirstOrDefaultAsync();
    }

    // Оптимизированный метод для учителя - загружаем только нужные данные
    public async Task<Domain.Entities.UserProfile> GetTeacherProfileAsync(Guid userId)
    {
        return await _context.UserProfiles
            .Include(up => up.TeacherStats)
            .Include(up => up.Settings)
            .Include(up => up.RecentActivities.OrderByDescending(ra => ra.Timestamp).Take(10))
            .Where(up => up.UserId == userId && up.Role == UserRole.teacher)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Domain.Entities.UserProfile>> GetLeaderboardAsync(int topCount)
    {
        return await _context.UserProfiles
            .Where(up => up.IsPublic && up.Role == UserRole.student)
            .Include(up => up.Stats)
            .OrderByDescending(up => up.Stats.TotalExperience)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.UserProfile>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        var query = _context.UserProfiles
            .Where(up => up.Role == UserRole.student);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(up => 
                up.UserName.Contains(searchTerm) || 
                (up.Bio != null && up.Bio.Contains(searchTerm)) ||
                (up.Email != null && up.Email.Contains(searchTerm)));
        }

        return await query
            .Include(up => up.Stats)
            .OrderByDescending(up => up.Role)
            .ThenByDescending(up => up.Stats != null ? up.Stats.TotalExperience : 0)
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
        Console.Write("userProfileId ");
        Console.WriteLine(userProfileId);
        return await _context.RecentProblems
            .Where(rp => rp.UserProfileId == userProfileId)
            .OrderByDescending(rp => rp.SolvedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> HasUserSolvedProblemAsync(Guid userProfileId, Guid problemId)
    {
        return await _context.RecentProblems
            .AnyAsync(rp => rp.UserProfileId == userProfileId && rp.ProblemId == problemId);
    }

    public async Task<(int TotalActivities, int TodayActivities)> GetActivityStatsAsync(Guid userProfileId)
    {
        var today = DateTime.UtcNow.Date;
        
        var totalActivities = await _context.RecentActivities
            .CountAsync(ra => ra.UserProfileId == userProfileId);
            
        var todayActivities = await _context.RecentActivities
            .CountAsync(ra => ra.UserProfileId == userProfileId && ra.Timestamp.Date == today);

        return (totalActivities, todayActivities);
    }

    public async Task UpdateTeacherStatsAsync(
        Guid userId, 
        int? createdTasks = null, 
        int? activeStudents = null, 
        double? averageRating = null, 
        int? totalSubmissions = null)
    {
        var profile = await _context.UserProfiles
            .Include(up => up.TeacherStats)
            .FirstOrDefaultAsync(up => up.UserId == userId && up.Role == UserRole.teacher);

        if (profile != null)
        {
            profile.UpdateTeacherStats(createdTasks, activeStudents, averageRating, totalSubmissions);
            _context.UserProfiles.Update(profile);
        }
    }
    
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.UserProfiles
            .AnyAsync(up => up.Email == email);
    }
    
    public async Task<Domain.Entities.UserProfile> GetByEmailAsync(string email)
    {
        return await _context.UserProfiles
            .Include(up => up.Stats)
            .Include(up => up.TeacherStats)
            .Include(up => up.Settings)
            .FirstOrDefaultAsync(up => up.Email == email);
    }

    public async Task AddBattleResultAsync(BattleResult battleResult)
    {
        await _context.BattleResults.AddAsync(battleResult);
    }
    public async Task<List<UserAchievement>> GetUserAchievementsAsync(Guid userProfileId)
    {
        return await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserProfileId == userProfileId)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();
    }

    public async Task<UserAchievement?> GetUserAchievementAsync(Guid userProfileId, Guid achievementId)
    {
        return await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .FirstOrDefaultAsync(ua => 
                ua.UserProfileId == userProfileId && 
                ua.AchievementId == achievementId);
    }

    public async Task AddUserAchievementAsync(UserAchievement userAchievement)
    {
        // Проверяем, нет ли уже такого достижения у пользователя
        var existing = await _context.UserAchievements
            .FirstOrDefaultAsync(ua => 
                ua.UserProfileId == userAchievement.UserProfileId && 
                ua.AchievementId == userAchievement.AchievementId);
        
        if (existing == null)
        {
            await _context.UserAchievements.AddAsync(userAchievement);
        }
        else
        {
            // Обновляем существующее достижение
            existing.UnlockedAt = userAchievement.UnlockedAt;
            existing.Progress = userAchievement.Progress;
            _context.UserAchievements.Update(existing);
        }
    }

    public async Task UpdateUserAchievementProgressAsync(Guid userProfileId, Guid achievementId, int progress)
    {
        var userAchievement = await _context.UserAchievements
            .FirstOrDefaultAsync(ua => 
                ua.UserProfileId == userProfileId && 
                ua.AchievementId == achievementId);
        
        if (userAchievement != null)
        {
            userAchievement.Progress = progress;
            
            // Получаем требование для достижения
            var achievement = await _context.Achievements
                .FirstOrDefaultAsync(a => a.Id == achievementId);
            
            // Если прогресс достиг требуемого значения - разблокируем
            if (achievement != null && progress >= achievement.RequiredValue && !userAchievement.IsUnlocked)
            {
                userAchievement.IsUnlocked = true;
                userAchievement.UnlockedAt = DateTime.UtcNow;
            }
            
            _context.UserAchievements.Update(userAchievement);
        }
    }

    public async Task<bool> HasAchievementAsync(Guid userProfileId, Guid achievementId)
    {
        return await _context.UserAchievements
            .AnyAsync(ua => 
                ua.UserProfileId == userProfileId && 
                ua.AchievementId == achievementId && 
                ua.IsUnlocked);
    }

    public async Task<Dictionary<AchievementCategory, int>> GetAchievementStatsAsync(Guid userProfileId)
    {
        var achievements = await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserProfileId == userProfileId && ua.IsUnlocked)
            .Select(ua => ua.Achievement.Category)
            .ToListAsync();
        
        return achievements
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<List<UserAchievement>> GetRecentAchievementsAsync(Guid userProfileId, int count = 10)
    {
        return await _context.UserAchievements
            .Include(ua => ua.Achievement)
            .Where(ua => ua.UserProfileId == userProfileId && ua.IsUnlocked)
            .OrderByDescending(ua => ua.UnlockedAt)
            .Take(count)
            .ToListAsync();
    }

  

    public async Task<List<Achievement>> GetEligibleAchievementsAsync(Guid userProfileId)
    {
        // Получаем статистику пользователя
        var userProfile = await _context.UserProfiles
            .Include(up => up.Stats)
            .Include(up => up.Achievements)
            .FirstOrDefaultAsync(up => up.Id == userProfileId);
        
        if (userProfile == null || userProfile.Stats == null)
            return new List<Achievement>();
        
        // Получаем все достижения
        var allAchievements = await _context.Achievements.ToListAsync();
        var userAchievements = userProfile.Achievements
            .Where(ua => ua.IsUnlocked)
            .Select(ua => ua.AchievementId)
            .ToHashSet();
        
        var eligibleAchievements = new List<Achievement>();
        
        foreach (var achievement in allAchievements)
        {
            // Пропускаем уже полученные достижения
            if (userAchievements.Contains(achievement.Id))
                continue;
            
            // Пропускаем секретные достижения
            if (achievement.IsSecret)
                continue;
            
            // Проверяем критерии
            var isEligible = achievement.Type switch
            {
                AchievementType.TotalProblemsSolved => userProfile.Stats.TotalProblemsSolved >= achievement.RequiredValue,
                AchievementType.EasyProblemsSolved => userProfile.Stats.EasyProblemsSolved >= achievement.RequiredValue,
                AchievementType.MediumProblemsSolved => userProfile.Stats.MediumProblemsSolved >= achievement.RequiredValue,
                AchievementType.HardProblemsSolved => userProfile.Stats.HardProblemsSolved >= achievement.RequiredValue,
                AchievementType.Wins => userProfile.Stats.Wins >= achievement.RequiredValue,
                AchievementType.TotalBattles => userProfile.Stats.TotalBattles >= achievement.RequiredValue,
                AchievementType.CurrentStreak => userProfile.Stats.CurrentStreak >= achievement.RequiredValue,
                AchievementType.MaxStreak => userProfile.Stats.MaxStreak >= achievement.RequiredValue,
                AchievementType.SuccessRate => userProfile.Stats.SuccessRate >= achievement.RequiredValue,
                AchievementType.TotalExperience => userProfile.Stats.TotalExperience >= achievement.RequiredValue,
                AchievementType.TotalSubmissions => userProfile.Stats.TotalSubmissions >= achievement.RequiredValue,
                _ => false
            };
            
            if (isEligible)
            {
                eligibleAchievements.Add(achievement);
            }
        }
        
        return eligibleAchievements;
    }

  
}