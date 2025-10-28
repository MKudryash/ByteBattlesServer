using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;

public interface ITaskServiceClient
{
    Task<TaskDto> GetTaskAsync(Guid taskId);
    Task<List<TestCaseDto>> GetTestCasesAsync(Guid taskId);
}