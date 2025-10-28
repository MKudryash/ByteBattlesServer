using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Repositories;


public class UserSolutionRepository : IUserSolutionRepository
{
    private readonly SolutionDbContext _context;

    public UserSolutionRepository(SolutionDbContext context)
    {
        _context = context;
    }

    // Basic CRUD operations
    public async Task<UserSolution?> GetByIdAsync(Guid id)
    {
        return await _context.UserSolutions
            .Include(s => s.Status) // Если есть навигационное свойство
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<UserSolution>> GetAllAsync()
    {
        return await _context.UserSolutions
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task AddAsync(UserSolution solution)
    {
        await _context.UserSolutions.AddAsync(solution);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserSolution solution)
    {
        _context.UserSolutions.Update(solution);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var solution = await GetByIdAsync(id);
        if (solution != null)
        {
            _context.UserSolutions.Remove(solution);
            await _context.SaveChangesAsync();
        }
    }

    // UserSolution query operations
    public async Task<IEnumerable<UserSolution>> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSolution>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.UserSolutions
            .Where(s => s.TaskId == taskId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSolution>> GetByUserIdAndTaskIdAsync(Guid userId, Guid taskId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId && s.TaskId == taskId)
            .OrderBy(s => s.AttemptNumber)
            .ToListAsync();
    }

    public async Task<UserSolution?> GetLastSolutionAsync(Guid userId, Guid taskId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId && s.TaskId == taskId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserSolution>> GetRecentSolutionsAsync(Guid userId, int count = 10)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserSolution>> GetSolutionsByStatusAsync(SolutionStatus status)
    {
        return await _context.UserSolutions
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    // Attempt number operations
    public async Task<int> GetNextAttemptNumberAsync(Guid userId, Guid taskId)
    {
        var lastAttempt = await _context.UserSolutions
            .Where(s => s.UserId == userId && s.TaskId == taskId)
            .OrderByDescending(s => s.AttemptNumber)
            .FirstOrDefaultAsync();

        return (lastAttempt?.AttemptNumber ?? 0) + 1;
    }

    public async Task<int> GetUserTotalAttemptsAsync(Guid userId, Guid taskId)
    {
        return await _context.UserSolutions
            .CountAsync(s => s.UserId == userId && s.TaskId == taskId);
    }

    public async Task<int> GetUserSuccessfulAttemptsAsync(Guid userId, Guid taskId)
    {
        return await _context.UserSolutions
            .CountAsync(s => s.UserId == userId && s.TaskId == taskId && s.IsCorrect);
    }

    // Statistics operations
    public async Task<int> GetUserTotalSolutionsCountAsync(Guid userId)
    {
        return await _context.UserSolutions
            .CountAsync(s => s.UserId == userId);
    }

    public async Task<int> GetUserSuccessfulSolutionsCountAsync(Guid userId)
    {
        return await _context.UserSolutions
            .CountAsync(s => s.UserId == userId && s.IsCorrect);
    }

    public async Task<Dictionary<Guid, int>> GetUserSolutionsByTaskAsync(Guid userId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.TaskId)
            .Select(g => new { TaskId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.TaskId, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetUserSolutionsByLanguageAsync(Guid userId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.Language)
            .Select(g => new { Language = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Language, x => x.Count);
    }

    // UserTaskStats operations
    public async Task<UserTaskStats?> GetUserTaskStatsAsync(Guid userId, Guid taskId)
    {
        return await _context.UserTaskStats
            .FirstOrDefaultAsync(s => s.UserId == userId && s.TaskId == taskId);
    }

    public async Task<IEnumerable<UserTaskStats>> GetUserStatsAsync(Guid userId)
    {
        return await _context.UserTaskStats
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task AddOrUpdateUserTaskStatsAsync(UserTaskStats stats)
    {
        var existingStats = await GetUserTaskStatsAsync(stats.UserId, stats.TaskId);
        
        if (existingStats == null)
        {
            await _context.UserTaskStats.AddAsync(stats);
        }
        else
        {
            _context.UserTaskStats.Update(stats);
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserTaskStatsAsync(UserTaskStats stats)
    {
        _context.UserTaskStats.Update(stats);
        await _context.SaveChangesAsync();
    }

    // Pagination and filtering
    public async Task<(IEnumerable<UserSolution> Solutions, int TotalCount)> GetSolutionsPagedAsync(
        Guid? userId = null, 
        Guid? taskId = null, 
        SolutionStatus? status = null,
        int pageNumber = 1, 
        int pageSize = 20)
    {
        var query = _context.UserSolutions.AsQueryable();

        if (userId.HasValue)
            query = query.Where(s => s.UserId == userId.Value);
        
        if (taskId.HasValue)
            query = query.Where(s => s.TaskId == taskId.Value);
        
        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        var totalCount = await query.CountAsync();
        
        var solutions = await query
            .OrderByDescending(s => s.SubmittedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (solutions, totalCount);
    }

    // Performance metrics
    public async Task<TimeSpan?> GetUserBestExecutionTimeAsync(Guid userId, Guid taskId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId && s.TaskId == taskId && s.IsCorrect && s.ExecutionTime > TimeSpan.Zero)
            .MinAsync(s => (TimeSpan?)s.ExecutionTime);
    }

    public async Task<double> GetUserSuccessRateAsync(Guid userId)
    {
        var total = await GetUserTotalSolutionsCountAsync(userId);
        var successful = await GetUserSuccessfulSolutionsCountAsync(userId);
        
        return total > 0 ? (double)successful / total * 100 : 0;
    }

    public async Task<DateTime?> GetFirstSolvedDateAsync(Guid userId, Guid taskId)
    {
        return await _context.UserSolutions
            .Where(s => s.UserId == userId && s.TaskId == taskId && s.IsCorrect)
            .OrderBy(s => s.CompletedAt)
            .Select(s => s.CompletedAt)
            .FirstOrDefaultAsync();
    }

    // Bulk operations
    public async Task AddRangeAsync(IEnumerable<UserSolution> solutions)
    {
        await _context.UserSolutions.AddRangeAsync(solutions);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserStatsForSolutionAsync(UserSolution solution)
    {
        var stats = await GetUserTaskStatsAsync(solution.UserId, solution.TaskId);
        
        if (stats == null)
        {
            stats = new UserTaskStats(solution.UserId, solution.TaskId);
            await _context.UserTaskStats.AddAsync(stats);
        }
        
        stats.RecordAttempt(solution);
        await _context.SaveChangesAsync();
    }
}