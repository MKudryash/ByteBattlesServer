using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Repositories;

public class SolutionRepository : ISolutionRepository
{
    private readonly SolutionDbContext _context;

    public SolutionRepository(SolutionDbContext context)
    {
        _context = context;
    }

    public async Task<Solution?> GetByIdAsync(Guid id)
    {
        return await _context.Solutions
            .Include(s => s.TestResults)
            .Include(s => s.Attempts)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Solution>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Solutions
            .Include(s => s.TestResults)
            .Include(s => s.Attempts)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Solution>> GetByTaskIdAsync(Guid taskId)
    {
        return await _context.Solutions
            .Include(s => s.TestResults)
            .Include(s => s.Attempts)
            .Where(s => s.TaskId == taskId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Solution>> GetByTaskAndUserAsync(Guid taskId, Guid userId)
    {
        return await _context.Solutions
            .Include(s => s.TestResults)
            .Include(s => s.Attempts)
            .Where(s => s.TaskId == taskId && s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Solution?> GetLastSolutionAsync(Guid taskId, Guid userId)
    {
        return await _context.Solutions
            .Include(s => s.TestResults)
            .Include(s => s.Attempts)
            .Where(s => s.TaskId == taskId && s.UserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .FirstOrDefaultAsync();
    }

    public async System.Threading.Tasks.Task AddAsync(Solution solution)
    {
        await _context.Solutions.AddAsync(solution);
    }

    public async System.Threading.Tasks.Task UpdateAsync(Solution solution)
    {
        _context.Solutions.Update(solution);
    }

    public async Task<List<SolutionAttempt>> GetAttemptsBySolutionIdAsync(Guid solutionId)
    {
        return await _context.SolutionAttempts
            .Where(a => a.SolutionId == solutionId)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task AddAttemptAsync(SolutionAttempt attempt)
    {
        await _context.SolutionAttempts.AddAsync(attempt);
    }

    public async Task<List<TestResult>> GetTestResultsBySolutionIdAsync(Guid solutionId)
    {
        return await _context.TestResults
            .Where(tr => tr.SolutionId == solutionId)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task AddTestResultAsync(TestResult testResult)
    {
        await _context.TestResults.AddAsync(testResult);
    }

    public async System.Threading.Tasks.Task AddTestResultsAsync(IEnumerable<TestResult> testResults)
    {
        await _context.TestResults.AddRangeAsync(testResults);
    }

    public async Task<SolutionStatistics> GetUserStatisticsAsync(Guid userId)
    {
        var solutions = await _context.Solutions
            .Where(s => s.UserId == userId)
            .ToListAsync();

        var totalSubmissions = solutions.Count;
        var successfulSubmissions = solutions.Count(s => s.Status == Domain.Enums.SolutionStatus.Completed);
        var successRate = totalSubmissions > 0 ? (double)successfulSubmissions / totalSubmissions * 100 : 0;
        
        var averageExecutionTime = solutions
            .Where(s => s.ExecutionTime.HasValue)
            .Average(s => s.ExecutionTime.Value.TotalMilliseconds);

        var favoriteLanguage = await _context.Solutions
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.LanguageId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key.ToString())
            .FirstOrDefaultAsync() ?? "Unknown";

        return new SolutionStatistics(totalSubmissions, successfulSubmissions, successRate, averageExecutionTime, favoriteLanguage);
    }

    public async Task<List<Solution>> GetRecentSolutionsAsync(int count)
    {
        return await _context.Solutions
            .Include(s => s.TestResults)
            .OrderByDescending(s => s.SubmittedAt)
            .Take(count)
            .ToListAsync();
    }
}