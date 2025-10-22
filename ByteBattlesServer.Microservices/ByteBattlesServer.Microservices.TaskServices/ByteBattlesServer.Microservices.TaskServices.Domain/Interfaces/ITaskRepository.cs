using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;

public interface ITaskRepository
{
    Task<Task> GetByIdAsync(Guid id);
    Task<Task> GetByTitleAsync(string title);
    Task<List<Task>> GetAllAsync();
    System.Threading.Tasks.Task AddAsync(Task task);
    System.Threading.Tasks.Task Update(Task task);
    System.Threading.Tasks.Task Delete(Task task);
    
    
    
    Task<List<TaskLanguage>> GetTaskLanguagesAsync(Guid taskId);
    void RemoveTaskLanguage(TaskLanguage taskLanguage);
    System.Threading.Tasks.Task AddTaskLanguageAsync(TaskLanguage taskLanguage);

    
    Task<List<TestCases>> GetTestCasesAsync(Guid taskId);
    Task<TestCases?> GetTestCaseByIdAsync(Guid testCaseId);
    System.Threading.Tasks.Task AddTestCaseAsync(TestCases testCase);
    System.Threading.Tasks.Task UpdateTestCaseAsync(TestCases testCase);
    System.Threading.Tasks.Task DeleteTestCaseAsync(TestCases testCase);
    System.Threading.Tasks.Task AddTestCasesAsync(IEnumerable<TestCases> testCases);
    
    
    
    Task<List<Task>> SearchTask(Difficulty? difficulty,
        Guid? languageId,
        string? searchTerm); 
    
    Task<List<Task>> SearchTasksPagedAsync(Difficulty? difficulty,
        Guid? languageId,
        string? searchTerm, int page, int pageSize);
}