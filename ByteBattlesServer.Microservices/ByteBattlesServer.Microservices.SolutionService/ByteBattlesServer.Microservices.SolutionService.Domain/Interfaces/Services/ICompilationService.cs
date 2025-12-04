using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface ICompilationService
{
    Task<List<TestExecutionResult>> ExecuteAllTestsAsync(string compiledCode, List<TestCaseDto> testCases,
        LanguageInfo languageId, List<LibraryInfo> libraries,string patternMain);
}