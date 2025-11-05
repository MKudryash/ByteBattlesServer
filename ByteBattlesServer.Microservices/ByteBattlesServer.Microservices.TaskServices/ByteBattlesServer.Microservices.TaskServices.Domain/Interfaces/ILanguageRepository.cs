using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;

public interface ILanguageRepository
{
    Task<Language> GetByIdAsync(Guid id);
    Task<Language> GetByNameAsync(string name);
    Task<List<TaskLanguage>> GetTaskLanguagesAsync(Guid taskId);
    
    Task<List<Language>> GetAllAsync();
    Task AddAsync(Language language);
    Task Update(Language language);
    Task Delete(Language language);
    Task<List<Language>> SearchLanguage(string searchTerm, int page, int pageSize);
    System.Threading.Tasks.Task<List<Language>> SearchLanguages(
        string? searchTerm); 
    
    System.Threading.Tasks.Task<List<Language>> SearchLanguagesPagedAsync(
        string? searchTerm, int page, int pageSize);
}