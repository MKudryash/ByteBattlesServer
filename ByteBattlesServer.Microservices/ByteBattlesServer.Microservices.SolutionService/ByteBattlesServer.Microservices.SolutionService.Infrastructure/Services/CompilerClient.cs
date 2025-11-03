using System.Text;
using System.Text.Json;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Services;


public class CompilerClient : ICompilerClient
{
    private readonly HttpClient _httpClient;
    private readonly CompilerServiceOptions _options;
    private readonly ILogger<CompilerClient> _logger;

    public CompilerClient(
        HttpClient httpClient, 
        IOptions<CompilerServiceOptions> options,
        ILogger<CompilerClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
        
        // Настройка базового URL из конфигурации
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }
    

   
    public async Task<BatchTestExecutionResult> ExecuteTestsAsync(string compiledCode, List<TestCase> testCases, Guid languageId)
    {
        try
        {
            var request = new 
            {
                Code = compiledCode,
                Language = 0,
                TestCases = testCases.Select(tc => new 
                {
                    Input = tc.Input,
                    ExpectedOutput = tc.ExpectedOutput
                })
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Executing batch of {TestCount} tests for language {LanguageId}", testCases.Count(), languageId);

            var response = await _httpClient.PostAsync("/api/compiler/test", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Batch test execution failed: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var testResult = JsonSerializer.Deserialize<CodeTestResultResponse>(
                responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var results = new List<TestExecutionResult>();
            
            if (testResult?.Results != null)
            {
                foreach (var (testCase, resultDto) in testCases.Zip(testResult.Results))
                {
                    results.Add(new TestExecutionResult(
                        resultDto.IsPassed,
                        resultDto.ActualOutput,
                        resultDto.IsPassed ? null : $"Expected: {resultDto.ExpectedOutput}, Actual: {resultDto.ActualOutput}",
                        resultDto.ExecutionTime,
                        0 // Memory usage not available
                    ));
                }
            }

            return new BatchTestExecutionResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing batch tests for language {LanguageId}", languageId);
            throw;
        }
    }
}

