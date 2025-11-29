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

    Task<Task> GetByIdAsyncWithTasks(Guid id);
    System.Threading.Tasks.Task<(int Easy, int Medium, int Hard)> TaskCountDiffaclty();

    Task<Task> GetRandomByDifficultyAsync(Difficulty difficulty);
    
    Task<List<TaskLanguage>> GetTaskLanguagesAsync(Guid taskId);
    Task<List<TaskLibrary>> GetTaskLibraryAsync(Guid taskId);
    void RemoveTaskLanguage(TaskLanguage taskLanguage);
    void RemoveTaskLibrary(TaskLibrary taskLibrary);
    System.Threading.Tasks.Task AddTaskLanguageAsync(TaskLanguage taskLanguage);
    System.Threading.Tasks.Task AddTaskLibraryAsync(TaskLibrary taskLibrary);

    
    Task<List<TestCases>> GetTestCasesAsync(Guid taskId);
    Task<TestCases?> GetTestCaseByIdAsync(Guid testCaseId);
    System.Threading.Tasks.Task AddTestCaseAsync(TestCases testCase);
    System.Threading.Tasks.Task UpdateTestCaseAsync(TestCases testCase);
    System.Threading.Tasks.Task RemoveTestCaseAsync(TestCases testCase);
    System.Threading.Tasks.Task AddTestCasesAsync(IEnumerable<TestCases> testCases);
    
    Task<Task?> InfoForCompilerAsync(Guid taskId);
    
    
    Task<List<Task>> SearchTask(Difficulty? difficulty,
        Guid? languageId,
        string? searchTerm); 
    
    Task<List<Task>> SearchTasksPagedAsync(Difficulty? difficulty,
        Guid? languageId,
        string? searchTerm, int page, int pageSize);
}