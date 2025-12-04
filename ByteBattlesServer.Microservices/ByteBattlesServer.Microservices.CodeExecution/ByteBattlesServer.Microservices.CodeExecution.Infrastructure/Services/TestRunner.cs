using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class TestRunner : ITestRunner
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly ICodeCompiler _codeCompiler;
    private readonly IFileService _fileService;

    private readonly ILogger<TestRunner> _logger;
    

    public TestRunner(ICodeGenerator codeGenerator, 
        ICodeCompiler codeCompiler, 
        IFileService fileService,
        IMessageBus messageBus,
        ILogger<TestRunner> logger)
    {
        _codeGenerator = codeGenerator;
        _codeCompiler = codeCompiler;
        _fileService = fileService;

        _logger = logger;
    }

    public async Task<TestResult> RunTestsAsync(CodeSubmission submission)
    {
        var results = new List<TestCaseResult>();
        var allTestsPassed = true;
        
        

        _logger.LogInformation(submission.Language.CompilerCommand);
        _logger.LogInformation(submission.Language.FileExtension);

        
        Console.WriteLine($"FileExtension C: {submission.Language.FileExtension}");
        
        var filePath = _fileService.GetTempFilePath(submission.Language.FileExtension);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        Console.WriteLine($"FileExtension Java: {fileNameWithoutExtension}");
        var executableCode = _codeGenerator.GenerateExecutableCode(submission,fileNameWithoutExtension);
        
        await _fileService.WriteToFileAsync(executableCode, filePath);
        

        try
        {
            // Компиляция (если требуется)
            if (submission.Language.SupportsCompilation)
            {
                _logger.LogInformation("Compiling code...");
                var compileResult = await _codeCompiler.CompileAsync(filePath, submission.Language);
                if (!compileResult.IsSuccess)
                {
                    return new TestResult(false, new List<TestCaseResult>(), $"Compilation failed: {compileResult.Output}");
                }
            }

            // Запуск тестов
            foreach (var testCase in submission.TestCases)
            {
                var executionResult = await _codeCompiler.ExecuteAsync(filePath, submission.Language, testCase.Input);
                    
                var isPassed = executionResult.IsSuccess && 
                               executionResult.Output.Trim() == testCase.ExpectedOutput.Trim();
                    
                if (!isPassed) allTestsPassed = false;

                results.Add(new TestCaseResult(testCase, executionResult.Output, isPassed, executionResult.ExecutionTime));
            }

            var summary = allTestsPassed 
                ? "All tests passed successfully" 
                : $"{results.Count(r => !r.IsPassed)} of {results.Count} tests failed";

            return new TestResult(allTestsPassed, results, summary);
        }
        finally
        {
            
            // Очистка временных файлов
            await _fileService.DeleteFileAsync(filePath);
        }
    }
    private string NormalizeCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return string.Empty;

        // Удаляем экранирование JSON/HTTP символов
        return code
            .Replace("\\\"", "\"")          // Экранированные кавычки
            .Replace("\\n", "\n")           // Переносы строк
            .Replace("\\t", "\t")           // Табуляции
            .Replace("\\r", "\r")           // Возврат каретки
            .Replace("\\\\", "\\")          // Обратные слеши
            .Replace("\\'", "'")            // Одиночные кавычки
            .Replace("\\b", "\b")           // Backspace
            .Replace("\\f", "\f")           // Form feed
            .Replace("\\/", "/")            // Слэши
            .Replace("\\u0022", "\"");      // Unicode кавычки
    }
}