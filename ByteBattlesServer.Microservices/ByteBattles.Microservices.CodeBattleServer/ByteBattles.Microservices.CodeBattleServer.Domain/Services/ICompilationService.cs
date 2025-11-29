using ByteBattles.Microservices.CodeBattleServer.Domain.Models;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattles.Microservices.CodeBattleServer.Domain.Services;

public interface ICompilationService
{
    Task<List<TestExecutionResult>> ExecuteAllTestsAsync(string compiledCode, List<TestCaseInfo> testCases,
        Guid languageId);
}