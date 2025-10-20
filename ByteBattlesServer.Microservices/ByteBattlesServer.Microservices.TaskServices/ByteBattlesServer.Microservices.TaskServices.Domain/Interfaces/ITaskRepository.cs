using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;

public interface ITaskRepository
{
    Task<Task> GetByIdAsync(Guid id);
    Task<List<Task>> GetAllAsync();
    Task AddAsync(Task task);
    Task Update(Task task);
    Task Delete(Guid id);
    Task SearchTask (string searchTerm, int page, int pageSize);
    
}