using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface ITestCasesServices
{
    Task<List<TestCaseInfo>> GetTestCasesInfoAsync(Guid taskId);
    
}
