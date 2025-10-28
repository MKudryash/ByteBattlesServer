using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;


public interface IUserSolutionRepository
{
   
    Task<UserSolution?> GetByIdAsync(Guid id);
    Task<IEnumerable<UserSolution>> GetAllAsync();
    Task AddAsync(UserSolution solution);
    Task UpdateAsync(UserSolution solution);
    Task DeleteAsync(Guid id);

   
    Task<IEnumerable<UserSolution>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<UserSolution>> GetByTaskIdAsync(Guid taskId);
    Task<IEnumerable<UserSolution>> GetByUserIdAndTaskIdAsync(Guid userId, Guid taskId);
    Task<UserSolution?> GetLastSolutionAsync(Guid userId, Guid taskId);
    Task<IEnumerable<UserSolution>> GetRecentSolutionsAsync(Guid userId, int count = 10);
    Task<IEnumerable<UserSolution>> GetSolutionsByStatusAsync(SolutionStatus status);
    
    
    Task<int> GetNextAttemptNumberAsync(Guid userId, Guid taskId);
    Task<int> GetUserTotalAttemptsAsync(Guid userId, Guid taskId);
    Task<int> GetUserSuccessfulAttemptsAsync(Guid userId, Guid taskId);
    
   
    
    Task<int> GetUserTotalSolutionsCountAsync(Guid userId);
    Task<int> GetUserSuccessfulSolutionsCountAsync(Guid userId);
    Task<Dictionary<Guid, int>> GetUserSolutionsByTaskAsync(Guid userId);
    Task<Dictionary<string, int>> GetUserSolutionsByLanguageAsync(Guid userId);
    
    
    
    Task<UserTaskStats?> GetUserTaskStatsAsync(Guid userId, Guid taskId);
    Task<IEnumerable<UserTaskStats>> GetUserStatsAsync(Guid userId);
    Task AddOrUpdateUserTaskStatsAsync(UserTaskStats stats);
    Task UpdateUserTaskStatsAsync(UserTaskStats stats);
    
   
    
    Task<(IEnumerable<UserSolution> Solutions, int TotalCount)> GetSolutionsPagedAsync(
        Guid? userId = null, 
        Guid? taskId = null, 
        SolutionStatus? status = null,
        int pageNumber = 1, 
        int pageSize = 20);
    
    
    
    Task<TimeSpan?> GetUserBestExecutionTimeAsync(Guid userId, Guid taskId);
    Task<double> GetUserSuccessRateAsync(Guid userId);
    Task<DateTime?> GetFirstSolvedDateAsync(Guid userId, Guid taskId);
    
   
    
    Task AddRangeAsync(IEnumerable<UserSolution> solutions);
    Task UpdateUserStatsForSolutionAsync(UserSolution solution);
}