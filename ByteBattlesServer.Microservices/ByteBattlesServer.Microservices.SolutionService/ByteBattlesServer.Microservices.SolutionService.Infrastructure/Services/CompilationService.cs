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
    
    
    public async Task<List<TestExecutionResult>> ExecuteAllTestsAsync(string compiledCode,
        List<TestCaseDto> testCases,
        Guid languageId)
    {
        _logger.LogInformation("Executing {TestCount} tests for language {LanguageId}", 
            testCases.Count(), languageId);

        var batchResult = await _compilerClient.ExecuteTestsAsync(compiledCode, 
            testCases.Select(x=> new TestCase(x.Id,x.Input,x.Output)).ToList(), languageId);
        
        _logger.LogInformation("Batch test execution completed. {PassedTests}/{TotalTests} tests passed", 
            batchResult.Results.Count(r => r.IsSuccess), testCases.Count());

        return batchResult.Results;
    }
}