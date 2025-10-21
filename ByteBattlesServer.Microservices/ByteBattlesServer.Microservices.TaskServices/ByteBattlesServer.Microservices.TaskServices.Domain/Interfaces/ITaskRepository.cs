using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;

public interface ITaskRepository
{
    System.Threading.Tasks.Task<Domain.Entities.Task> GetByIdAsync(Guid id);
    System.Threading.Tasks.Task<Task> GetByTitleAsync(string title);
    
    System.Threading.Tasks.Task<List<Task>> GetAllAsync();
    System.Threading.Tasks.Task AddAsync(Task task);
    System.Threading.Tasks.Task Update(Task task);
    System.Threading.Tasks.Task Delete(Task task);

    System.Threading.Tasks.Task<List<Task>> SearchTask(Difficulty difficulty,
        Guid languageId,
        string searchTerm, int page, int pageSize);
}