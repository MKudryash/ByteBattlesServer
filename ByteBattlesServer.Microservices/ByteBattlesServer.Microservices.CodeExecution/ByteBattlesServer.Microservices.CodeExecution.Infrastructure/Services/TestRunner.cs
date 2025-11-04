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
    private readonly ILanguageService _languageService;
    private readonly ILogger<TestRunner> _logger;
    

    public TestRunner(ICodeGenerator codeGenerator, 
        ICodeCompiler codeCompiler, 
        IFileService fileService,
        IMessageBus messageBus,
        ILanguageService languageService,
        ILogger<TestRunner> logger)
    {
        _codeGenerator = codeGenerator;
        _codeCompiler = codeCompiler;
        _fileService = fileService;
        _languageService = languageService;
        _logger = logger;
    }

    public async Task<TestResult> RunTestsAsync(CodeSubmission submission)
    {
        var results = new List<TestCaseResult>();
        var allTestsPassed = true;

        // Генерация исполняемого кода
        //var executableCode = _codeGenerator.GenerateExecutableCode(submission);
        var executableCode = submission.Code;

        //Получить информацию о языке submission.Language
        var languageInfo = await _languageService.GetLanguageInfoAsync(submission.Language);
        _logger.LogInformation(languageInfo.CompilerCommand);
        _logger.LogInformation(languageInfo.FileExtension);
        // var languageInfo = new LanguageInfo()
        // {
        //     ShortTitle = "C",
        //     ExecutionCommand = ".c",
        //     SupportsCompilation = true
        // };
 
        
        
        // Создание временного файла
        var filePath = _fileService.GetTempFilePath(languageInfo.FileExtension);
        await _fileService.WriteToFileAsync(executableCode, filePath);

        try
        {
            // Компиляция (если требуется)
            if (languageInfo.SupportsCompilation)
            {
                _logger.LogInformation("Compiling code...");
                var compileResult = await _codeCompiler.CompileAsync(filePath, languageInfo);
                if (!compileResult.IsSuccess)
                {
                    return new TestResult(false, new List<TestCaseResult>(), $"Compilation failed: {compileResult.Output}");
                }
            }

            // Запуск тестов
            foreach (var testCase in submission.TestCases)
            {
                var executionResult = await _codeCompiler.ExecuteAsync(filePath, languageInfo, testCase.Input);
                    
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
}