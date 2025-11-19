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
        
        return await query
            .Include(l => l.Libraries)
            .ToListAsync();
    }

    public async Task<List<Language>> SearchLanguagesPagedAsync(string? searchTerm, int page, int pageSize)
    {
        var query = BuildSearchQuery( searchTerm);

        
        return await query
            .Include(l=>l.Libraries)
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
     public async Task<Library> GetLibraryByIdAsync(Guid libraryId)
    {
        return await _dbContext.Libraries
            .Include(l => l.Language)
            .FirstOrDefaultAsync(l => l.Id == libraryId);
    }

    public async Task<List<Library>> GetLibrariesByLanguageIdAsync(Guid languageId)
    {
        return await _dbContext.Libraries
            .Where(l => l.LanguageId == languageId)
            .Include(l => l.Language)
            .ToListAsync();
    }

    public async Task<List<Library>> GetLibrariesByLanguageNameAsync(string languageName)
    {
        return await _dbContext.Libraries
            .Where(l => l.Language.Title == languageName)
            .Include(l => l.Language)
            .ToListAsync();
    }

    public async Task<List<Library>> GetAllLibrariesAsync()
    {
        return await _dbContext.Libraries
            .Include(l => l.Language)
            .ToListAsync();
    }

    public async Task<List<Library>> SearchLibrariesAsync(string? searchTerm)
    {
        var query = BuildLibrarySearchQuery(searchTerm);
        return await query
            .Include(l => l.Language)
            .ToListAsync();
    }

    public async Task<List<Library>> SearchLibrariesPagedAsync(string? searchTerm, int page, int pageSize)
    {
        var query = BuildLibrarySearchQuery(searchTerm);
        return await query
            .Include(l => l.Language)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddLibraryAsync(Library library)
    {
        await _dbContext.Libraries.AddAsync(library);
    }

    public async Task UpdateLibraryAsync(Library library)
    {
        
        _dbContext.Libraries.Update(library);
    }

    public async Task DeleteLibraryAsync(Library library)
    {
        _dbContext.Libraries.Remove(library);
    }

    public async Task<bool> LibraryExistsAsync(string libraryName, string version, Guid languageId)
    {
        return await _dbContext.Libraries
            .AnyAsync(l => l.NameLibrary == libraryName && 
                          l.Version == version && 
                          l.LanguageId == languageId);
    }

    private IQueryable<Library> BuildLibrarySearchQuery(string? searchTerm)
    {
        var query = _dbContext.Libraries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(l => 
                l.NameLibrary.Contains(searchTerm) ||
                l.Description.Contains(searchTerm) ||
                l.Version.Contains(searchTerm) ||
                l.Language.Title.Contains(searchTerm));
        }

        return query;
    }
}