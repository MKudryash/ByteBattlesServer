using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;

public interface ICompilationService
{
    Task<CompilationResult> CompileAsync(string code, Guid languageId, string functionName);
    Task<TestExecutionResult> ExecuteTestAsync(string compiledCode, TestCase testCase, Guid languageId);
}