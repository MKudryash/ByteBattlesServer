using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;


public interface ICompilerClient
{
    Task<CompilationResult> CompileAsync(string code, Guid languageId, string functionName);
    Task<TestExecutionResult> ExecuteTestAsync(string compiledCode, TestCase testCase, Guid languageId);
    Task<BatchTestExecutionResult> ExecuteTestsAsync(string compiledCode, IEnumerable<TestCase> testCases, Guid languageId);
}


// Модели для работы с компилятором
public record CompilationRequest(string Code, Guid LanguageId, string FunctionName);
public record CompilationResponse(bool IsSuccess, string? CompiledCode, string? ErrorMessage, TimeSpan CompilationTime);

public record TestExecutionRequest(string CompiledCode, string Input, Guid LanguageId);
public record TestExecutionResponse(bool IsSuccess, string? Output, string? ErrorMessage, TimeSpan ExecutionTime, int MemoryUsed);

public record BatchTestExecutionRequest(string CompiledCode, IEnumerable<TestExecutionRequest> Tests, Guid LanguageId);
public record BatchTestExecutionResponse(List<TestExecutionResponse> Results);

// Модель тест-кейса для компиляции
public record TestCase(Guid Id, string Input, string ExpectedOutput);

// Конфигурация для сервиса компиляции
public class CompilerServiceOptions
{
    public string BaseUrl { get; set; } = "http://compiler-service:8080";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}

// Результат пакетного выполнения тестов
public record BatchTestExecutionResult(List<TestExecutionResult> Results);