using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface ITaskInfoServices
{
    Task<TaskInfo> GetTaskInfoAsync(Guid taskId);
}