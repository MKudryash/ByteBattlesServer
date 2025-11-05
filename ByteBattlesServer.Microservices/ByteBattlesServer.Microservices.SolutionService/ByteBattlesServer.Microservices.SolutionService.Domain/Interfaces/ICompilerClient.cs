using ByteBattlesServer.Microservices.SolutionService.Domain.Models;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Interfaces;


public interface ICompilerClient
{
    Task<BatchTestExecutionResult> ExecuteTestsAsync(string compiledCode, List<TestCase> testCases, Guid languageId);
}

public class CodeTestResultResponse
{
    public bool AllTestsPassed { get; set; }
    public List<TestCaseResultDto> Results { get; set; }
    public string Summary { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
}

public class TestCaseResultDto
{
    public string Input { get; set; }
    public string ExpectedOutput { get; set; }
    public string ActualOutput { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}

// Модель тест-кейса для компиляции
public record TestCase(Guid Id, string Input, string ExpectedOutput);

// Конфигурация для сервиса компиляции
public class CompilerServiceOptions
{
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
}

// Результат пакетного выполнения тестов
public record BatchTestExecutionResult(List<TestExecutionResult> Results);