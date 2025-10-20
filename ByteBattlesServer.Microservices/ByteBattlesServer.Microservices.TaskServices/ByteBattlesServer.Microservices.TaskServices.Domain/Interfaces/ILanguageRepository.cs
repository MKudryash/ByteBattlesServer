using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;

public interface ILanguageRepository
{
    Task<Language> GetByIdAsync(Guid id);
    Task<List<Language>> GetAllAsync();
    Task AddAsync(Language language);
    Task Update(Language language);
    Task Delete(Guid id);
    Task SearchLanguage(string searchTerm, int page, int pageSize);
}