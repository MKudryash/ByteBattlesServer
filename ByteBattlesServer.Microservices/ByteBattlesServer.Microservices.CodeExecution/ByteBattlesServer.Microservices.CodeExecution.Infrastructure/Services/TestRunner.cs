using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class TestRunner : ITestRunner
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly ICodeCompiler _codeCompiler;
    private readonly IFileService _fileService;

    public TestRunner(ICodeGenerator codeGenerator, ICodeCompiler codeCompiler, IFileService fileService)
    {
        _codeGenerator = codeGenerator;
        _codeCompiler = codeCompiler;
        _fileService = fileService;
    }

    public async Task<TestResult> RunTestsAsync(CodeSubmission submission)
    {
        var results = new List<TestCaseResult>();
        var allTestsPassed = true;

        // Генерация исполняемого кода
        //var executableCode = _codeGenerator.GenerateExecutableCode(submission);
        var executableCode = submission.Code;

       // var test = _codeGenerator.GetFileExtension();
        // Создание временного файла
        var filePath = _fileService.GetTempFilePath(".c");
        await _fileService.WriteToFileAsync(executableCode, filePath);

        try
        {
            // Компиляция (если требуется)
            if (submission.Language == ProgrammingLanguage.C || submission.Language == ProgrammingLanguage.CSharp)
            {
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
}