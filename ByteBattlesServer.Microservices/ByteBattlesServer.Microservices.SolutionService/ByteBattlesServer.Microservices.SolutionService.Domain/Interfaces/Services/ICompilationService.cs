using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface ICompilationService
{
    Task<List<TestExecutionResult>> ExecuteAllTestsAsync(string compiledCode, List<TestCaseDto> testCases,
        Guid languageId);
}