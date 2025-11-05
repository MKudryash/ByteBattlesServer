using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using ByteBattlesServer.Microservices.TaskServices.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Repositories;

public class LanguageRepository:ILanguageRepository
{
    private readonly TaskDbContext _dbContext;

    public LanguageRepository(TaskDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Language> GetByIdAsync(Guid id)
    {
        return await _dbContext.Language.FirstOrDefaultAsync(up => up.Id == id);
    }

    public async Task<Language> GetByNameAsync(string name)
    {
        return await _dbContext.Language.FirstOrDefaultAsync(up => up.Title == name);

    }

    public async Task<List<TaskLanguage>> GetTaskLanguagesAsync(Guid taskId)
    {
        return await _dbContext.TaskLanguages
            .Where(tl => tl.IdTask == taskId)
            .Include(tl => tl.Language)
            .ToListAsync();
    }

    public Task<List<Language>> GetAllAsync()
    {
        return _dbContext.Language.ToListAsync();
    }

    public async Task AddAsync(Language language)
    {
      await _dbContext.Language.AddAsync(language);
    }

    public async Task Update(Language language)
    {
         _dbContext.Language.Update(language);
    }

    public async Task Delete(Language language)
    {
        _dbContext.Language.Remove(language);
    }

    public async Task<List<Language>> SearchLanguage(string searchTerm, int page, int pageSize)
    {
        return await _dbContext.Language
            .Where(up => 
                         up.Title.Contains(searchTerm) || up.ShortTitle.Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Language>> SearchLanguages(string? searchTerm)
    {

        var query = BuildSearchQuery(searchTerm);
        
        return await query.ToListAsync();
    }

    public async Task<List<Language>> SearchLanguagesPagedAsync(string? searchTerm, int page, int pageSize)
    {
        var query = BuildSearchQuery( searchTerm);

        
        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    private IQueryable<Language> BuildSearchQuery(
        string? searchTerm)
    {
        var query = _dbContext.Language.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => 
                t.Title.Contains(searchTerm)||
                t.ShortTitle.Contains(searchTerm));
        }

        return query;
    }
}