using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Repository;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces.Services;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;


public class CompilationService : ICompilationService
{
    private readonly ICompilerClient _compilerClient;
    private readonly ILogger<CompilationService> _logger;

    public CompilationService(
        ICompilerClient compilerClient,
        ILogger<CompilationService> logger)
    {
        _compilerClient = compilerClient;
        _logger = logger;
    }

    public async Task<CompilationResult> CompileAsync(string code, Guid languageId, string functionName)
    {
        _logger.LogInformation("Compiling code for language {LanguageId}, function: {FunctionName}", 
            languageId, functionName);
        
        var result = await _compilerClient.CompileAsync(code, languageId, functionName);
        
        _logger.LogInformation("Compilation completed. Success: {IsSuccess}, Time: {CompilationTime}ms", 
            result.IsSuccess, result.CompilationTime.TotalMilliseconds);
            
        return result;
    }

    public async Task<TestExecutionResult> ExecuteTestAsync(string compiledCode, TestCase testCase, Guid languageId)
    {
        _logger.LogInformation("Executing test {TestCaseId} for language {LanguageId}", 
            testCase.Id, languageId);
        
        var result = await _compilerClient.ExecuteTestAsync(compiledCode, testCase, languageId);
        
        _logger.LogInformation("Test execution completed. Success: {IsSuccess}, Time: {ExecutionTime}ms", 
            result.IsSuccess, result.ExecutionTime.TotalMilliseconds);
            
        return result;
    }

    // Оптимизированный метод для выполнения всех тестов пакетно
    public async Task<List<TestExecutionResult>> ExecuteAllTestsAsync(
        string compiledCode, 
        IEnumerable<TestCase> testCases, 
        Guid languageId)
    {
        _logger.LogInformation("Executing {TestCount} tests for language {LanguageId}", 
            testCases.Count(), languageId);

        var batchResult = await _compilerClient.ExecuteTestsAsync(compiledCode, testCases, languageId);
        
        _logger.LogInformation("Batch test execution completed. {PassedTests}/{TotalTests} tests passed", 
            batchResult.Results.Count(r => r.IsSuccess), testCases.Count());

        return batchResult.Results;
    }
}