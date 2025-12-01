using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using ByteBattlesServer.Microservices.TaskServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _dbContext;

    public TaskRepository(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Task> GetRandomByDifficultyAsync(Difficulty difficulty,Guid languageId)
    {
        return await _dbContext.Tasks
            .Where(t => t.Difficulty == difficulty)
            .Include(t => t.TaskLanguages)
            .ThenInclude(tl => tl.Language)
            .Include(t => t.Libraries)
            .ThenInclude(l => l.Library)
            .Include(t => t.TestCases)
            .OrderBy(t => EF.Functions.Random())
            .Where(t=> t.TaskLanguages.Where(tl => tl.Language.Id == languageId).Any())
            .FirstOrDefaultAsync();
    }
    public async Task<Task> GetByIdAsync(Guid id)
    {
        return await _dbContext.Tasks
            .Include(up => up.TaskLanguages)
            .ThenInclude(ua => ua.Language)
            .Include(up => up.Libraries)
            .ThenInclude(l => l.Library)
            .Include(t => t.TestCases)
            .FirstOrDefaultAsync(up => up.Id == id);
    }  
    
    public async Task<Task> GetByIdAsyncWithTasks(Guid id)
    {
        return await _dbContext.Tasks
            .Include(up => up.TaskLanguages)
            .ThenInclude(ua => ua.Language)
            .Include(up => up.Libraries)
            .ThenInclude(l => l.Library)
            .Include(t => t.TestCases)
            .FirstOrDefaultAsync(up => up.Id == id);
    }
    public void RemoveTaskLanguage(TaskLanguage taskLanguage)
    {
        _dbContext.TaskLanguages.Remove(taskLanguage);
    }

    public void RemoveTaskLibrary(TaskLibrary taskLibrary)
    {
        _dbContext.TaskLibraries.Remove(taskLibrary);
    }

    public async Task<Task> GetByTitleAsync(string title)
    {
        return await _dbContext.Tasks
            .Include(up => up.TaskLanguages)
            .ThenInclude(ua => ua.Language)
            .Include(up => up.Libraries)
            .ThenInclude(l => l.Library)
            .Include(t => t.TestCases.Where(tc => tc.IsExample))
            .FirstOrDefaultAsync(up => up.Title == title);
    }

    public async Task<List<Task>> GetAllAsync()
    {
        return await _dbContext.Tasks
            .Include(t => t.TaskLanguages)
            .ThenInclude(tl => tl.Language)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task AddAsync(Task task)
    {
        await _dbContext.Tasks.AddAsync(task);
    }

    public async System.Threading.Tasks.Task Update(Task task)
    {
        _dbContext.Tasks.Update(task);
    }

    public async System.Threading.Tasks.Task Delete(Task task)
    {
        _dbContext.Tasks.Remove(task);
    }

    public async System.Threading.Tasks.Task<(int Easy, int Medium, int Hard)> TaskCountDiffaclty()
    {
        var counts = await _dbContext.Tasks
            .GroupBy(x => x.Difficulty)
            .Select(g => new { Difficulty = g.Key, Count = g.Count() })
            .ToListAsync();

        var easyCount = counts.FirstOrDefault(x => x.Difficulty.ToString() == "Easy")?.Count ?? 0;
        var mediumCount = counts.FirstOrDefault(x => x.Difficulty.ToString() == "Medium")?.Count ?? 0;
        var hardCount = counts.FirstOrDefault(x => x.Difficulty.ToString() == "Hard")?.Count ?? 0;
    
        return (easyCount, mediumCount, hardCount);
    }

    public async Task<List<TaskLanguage>> GetTaskLanguagesAsync(Guid taskId)
    {
        return await _dbContext.TaskLanguages
            .Where(tl => tl.IdTask == taskId)
            .Include(tl => tl.Language)
            .ToListAsync();
    }

    public async Task<List<TaskLibrary>> GetTaskLibraryAsync(Guid taskId)
    {
        return await _dbContext.TaskLibraries
            .Where(tl => tl.IdTask == taskId)
            .Include(tl => tl.Library)
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task AddTaskLanguageAsync(TaskLanguage taskLanguage)
    {
        await _dbContext.TaskLanguages.AddAsync(taskLanguage);
    }

    public async System.Threading.Tasks.Task AddTaskLibraryAsync(TaskLibrary taskLibrary)
    {
        await _dbContext.TaskLibraries.AddAsync(taskLibrary);
    }


    public async Task<Task?> InfoForCompilerAsync(Guid taskId)
    {
      return  await _dbContext.Tasks
            .Include(up => up.TaskLanguages)
            .Where(t=>t.Id == taskId)
            .Include(t=>t.TestCases)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Task>> SearchTask(Difficulty? difficulty, Guid? languageId, string? searchTerm)
    {
        var query = BuildSearchQuery(difficulty, languageId, searchTerm);
        
        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Task>> SearchTasksPagedAsync(Difficulty? difficulty, Guid? languageId, string? searchTerm, int page, int pageSize)
    {
        var query = BuildSearchQuery(difficulty, languageId, searchTerm);

        
        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    private IQueryable<Task> BuildSearchQuery(
        Difficulty? difficulty,
        Guid? languageId,
        string? searchTerm)
    {
        var query = _dbContext.Tasks
            .Include(t => t.TaskLanguages)
            .ThenInclude(tl => tl.Language)
            .Include(up => up.Libraries)
            .ThenInclude(l => l.Library)
            .Include(t => t.TestCases)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => 
                t.Title.Contains(searchTerm) ||
                t.Description.Contains(searchTerm) ||
                t.Author.Contains(searchTerm));
        }

        if (difficulty.HasValue)
        {
            query = query.Where(t => t.Difficulty == difficulty.Value);
        }

        if (languageId.HasValue)
        {
            query = query.Where(t => t.TaskLanguages.Any(tl => tl.IdLanguage == languageId.Value));
        }

        return query;
    }
    public async System.Threading.Tasks.Task<List<TestCases>> GetTestCasesAsync(Guid taskId)
    {
        return await _dbContext.TestsTasks
            .Where(tt => tt.TaskId == taskId)
            .OrderBy(tt => tt.CreatedAd)
            .ToListAsync();
    }

    public async Task<TestCases?> GetTestCaseByIdAsync(Guid testCaseId)
    {
        return await _dbContext.TestsTasks
            .Include(tt => tt.Task)
            .FirstOrDefaultAsync(tt => tt.Id == testCaseId);
    }

    public async System.Threading.Tasks.Task AddTestCaseAsync(TestCases testCase)
    {
        await _dbContext.TestsTasks.AddAsync(testCase);
    }

    public async System.Threading.Tasks.Task UpdateTestCaseAsync(TestCases testCase)
    {
        _dbContext.TestsTasks.Update(testCase);
    }

    public async System.Threading.Tasks.Task RemoveTestCaseAsync(TestCases testCase)
    {
        _dbContext.TestsTasks.Remove(testCase);
    }

    public async System.Threading.Tasks.Task AddTestCasesAsync(IEnumerable<TestCases> testCases)
    {
        await _dbContext.TestsTasks.AddRangeAsync(testCases);
    }

   
}