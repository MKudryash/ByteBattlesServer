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

    public async Task<Task> GetByIdAsync(Guid id)
    {
        return await _dbContext.Tasks
            .Include(up => up.TaskLanguages)
            .ThenInclude(ua => ua.Language)
            .FirstOrDefaultAsync(up => up.Id == id);
    }
    public void RemoveTaskLanguage(TaskLanguage taskLanguage)
    {
        _dbContext.TaskLanguages.Remove(taskLanguage);
    }
    public async Task<Task> GetByTitleAsync(string title)
    {
        return await _dbContext.Tasks
            .Include(up => up.TaskLanguages)
            .ThenInclude(ua => ua.Language)
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

    public async Task<List<TaskLanguage>> GetTaskLanguagesAsync(Guid taskId)
    {
        return await _dbContext.TaskLanguages
            .Where(tl => tl.IdTask == taskId)
            .Include(tl => tl.Language)
            .ToListAsync();
    }
    public async System.Threading.Tasks.Task AddTaskLanguageAsync(TaskLanguage taskLanguage)
    {
        await _dbContext.TaskLanguages.AddAsync(taskLanguage);
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

   
}