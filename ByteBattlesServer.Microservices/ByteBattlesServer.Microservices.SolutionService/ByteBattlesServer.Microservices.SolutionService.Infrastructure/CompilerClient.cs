using System.Text;
using System.Text.Json;
using ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;
using ByteBattlesServer.Microservices.SolutionService.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure;


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

    public async Task<CompilationResult> CompileAsync(string code, Guid languageId, string functionName)
    {
        try
        {
            var request = new CompilationRequest(code, languageId, functionName);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending compilation request for language {LanguageId}", languageId);

            var response = await _httpClient.PostAsync("/api/compiler/compile", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var compilationResponse = JsonSerializer.Deserialize<CompilationResponse>(
                responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (compilationResponse == null)
                throw new InvalidOperationException("Invalid response from compiler service");

            return new CompilationResult(
                compilationResponse.IsSuccess,
                compilationResponse.CompiledCode,
                compilationResponse.ErrorMessage,
                compilationResponse.CompilationTime);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error calling compiler service for language {LanguageId}", languageId);
            return new CompilationResult(false, null, $"Compiler service unavailable: {ex.Message}", TimeSpan.Zero);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during compilation for language {LanguageId}", languageId);
            return new CompilationResult(false, null, $"Compilation failed: {ex.Message}", TimeSpan.Zero);
        }
    }

    public async Task<TestExecutionResult> ExecuteTestAsync(string compiledCode, TestCase testCase, Guid languageId)
    {
        try
        {
            var request = new TestExecutionRequest(compiledCode, testCase.Input, languageId);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Executing test {TestCaseId} for language {LanguageId}", testCase.Id, languageId);

            var response = await _httpClient.PostAsync("/api/compiler/execute", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var executionResponse = JsonSerializer.Deserialize<TestExecutionResponse>(
                responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (executionResponse == null)
                throw new InvalidOperationException("Invalid response from compiler service");

            return new TestExecutionResult(
                executionResponse.IsSuccess,
                executionResponse.Output,
                executionResponse.ErrorMessage,
                executionResponse.ExecutionTime,
                executionResponse.MemoryUsed);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error executing test {TestCaseId} for language {LanguageId}", testCase.Id, languageId);
            return new TestExecutionResult(false, null, $"Test execution service unavailable: {ex.Message}", TimeSpan.Zero, 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during test execution {TestCaseId}", testCase.Id);
            return new TestExecutionResult(false, null, $"Test execution failed: {ex.Message}", TimeSpan.Zero, 0);
        }
    }

    public async Task<BatchTestExecutionResult> ExecuteTestsAsync(string compiledCode, IEnumerable<TestCase> testCases, Guid languageId)
    {
        try
        {
            var testRequests = testCases.Select(tc => new TestExecutionRequest(compiledCode, tc.Input, languageId)).ToList();
            var request = new BatchTestExecutionRequest(compiledCode, testRequests, languageId);
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Executing batch of {TestCount} tests for language {LanguageId}", testCases.Count(), languageId);

            var response = await _httpClient.PostAsync("/api/compiler/execute-batch", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var batchResponse = JsonSerializer.Deserialize<BatchTestExecutionResponse>(
                responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (batchResponse == null)
                throw new InvalidOperationException("Invalid response from compiler service");

            var results = batchResponse.Results.Select(r => new TestExecutionResult(
                r.IsSuccess,
                r.Output,
                r.ErrorMessage,
                r.ExecutionTime,
                r.MemoryUsed
            )).ToList();

            return new BatchTestExecutionResult(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing batch tests for language {LanguageId}", languageId);
            throw;
        }
    }
}

