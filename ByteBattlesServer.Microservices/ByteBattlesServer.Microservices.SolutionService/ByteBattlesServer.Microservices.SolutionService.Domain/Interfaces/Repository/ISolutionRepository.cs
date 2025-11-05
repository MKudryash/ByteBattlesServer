using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;


public interface ISolutionRepository
{
    Task<Solution?> GetByIdAsync(Guid id);
    Task<List<Solution>> GetByUserIdAsync(Guid userId);
    Task<List<Solution>> GetByTaskIdAsync(Guid taskId);
    Task<List<Solution>> GetByTaskAndUserAsync(Guid taskId, Guid userId);
    Task<Solution?> GetLastSolutionAsync(Guid taskId, Guid userId);
    System.Threading.Tasks.Task AddAsync(Solution solution);
    System.Threading.Tasks.Task UpdateAsync(Solution solution);
    
    // Attempts
    Task<List<SolutionAttempt>> GetAttemptsBySolutionIdAsync(Guid solutionId);
    System.Threading.Tasks.Task AddAttemptAsync(SolutionAttempt attempt);
    
    // Test Results
    Task<List<TestResult>> GetTestResultsBySolutionIdAsync(Guid solutionId);
    System.Threading.Tasks.Task AddTestResultAsync(TestResult testResult);
    System.Threading.Tasks.Task AddTestResultsAsync(IEnumerable<TestResult> testResults);
    
    // Statistics
    Task<SolutionStatistics> GetUserStatisticsAsync(Guid userId);
    Task<List<Solution>> GetRecentSolutionsAsync(int count);
}



// DTOs and Models