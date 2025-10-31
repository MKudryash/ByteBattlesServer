using System.Diagnostics;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class ProcessService : ICodeCompiler
{
    private readonly ILogger<ProcessService> _logger;

    public ProcessService(ILogger<ProcessService> logger)
    {
        _logger = logger;
    }

    public async Task<CodeExecutionResult> CompileAsync(string filePath, ProgrammingLanguage language)
    {
        _logger.LogInformation("Starting compilation for {Language} file: {FilePath}", language, filePath);
        
        try
        {
            var startInfo = CreateCompileProcessInfo(filePath, language);
            _logger.LogDebug("Compile command: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);
            
            return await ExecuteProcessAsync(startInfo, "compilation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compilation failed for {Language} file: {FilePath}", language, filePath);
            throw;
        }
    }

    public async Task<CodeExecutionResult> ExecuteAsync(string filePath, ProgrammingLanguage language, string arguments)
    {
        _logger.LogInformation("Starting execution for {Language} file: {FilePath} with arguments: {Arguments}", 
            language, filePath, arguments);
        
        try
        {
            var startInfo = CreateExecuteProcessInfo(filePath, language, arguments);
            _logger.LogDebug("Execute command: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);
            
            return await ExecuteProcessAsync(startInfo, "execution");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Execution failed for {Language} file: {FilePath} with arguments: {Arguments}", 
                language, filePath, arguments);
            throw;
        }
    }

    private ProcessStartInfo CreateCompileProcessInfo(string filePath, ProgrammingLanguage language)
    {
        var outputFileName = Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath));
        
        _logger.LogDebug("Generated output file name: {OutputFileName}", outputFileName);
        
        return language switch
        {
            ProgrammingLanguage.C => new ProcessStartInfo
            {
                FileName = "gcc",
                Arguments = $"{filePath} -o {outputFileName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            ProgrammingLanguage.CSharp => new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {filePath} -o {Path.GetDirectoryName(filePath)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            ProgrammingLanguage.Cpp => new ProcessStartInfo
            {
                FileName = "g++",
                Arguments = $"{filePath} -o {outputFileName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            _ => throw new NotSupportedException($"Language {language} is not supported for compilation")
        };
    }

    private ProcessStartInfo CreateExecuteProcessInfo(string filePath, ProgrammingLanguage language, string arguments)
    {
        var outputFileName = Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath));
        
        _logger.LogDebug("Execution - File: {FilePath}, Output: {OutputFileName}, Args: {Arguments}", 
            filePath, outputFileName, arguments);

        return language switch
        {
            ProgrammingLanguage.C => new ProcessStartInfo
            {
                FileName = outputFileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            ProgrammingLanguage.Cpp => new ProcessStartInfo
            {
                FileName = outputFileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            ProgrammingLanguage.CSharp => new ProcessStartInfo
            {
                FileName = "dotnet-exec",
                Arguments = $"{outputFileName}.cs --args \"{arguments}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            ProgrammingLanguage.Python => new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{filePath} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            ProgrammingLanguage.Java => new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"-cp {Path.GetDirectoryName(filePath)} {Path.GetFileNameWithoutExtension(filePath)} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            _ => throw new NotSupportedException($"Language {language} is not supported for execution")
        };
    }

    private async Task<CodeExecutionResult> ExecuteProcessAsync(ProcessStartInfo startInfo, string operationType)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogDebug("Starting process for {OperationType}: {FileName} {Arguments}", 
            operationType, startInfo.FileName, startInfo.Arguments);

        using var process = new Process { StartInfo = startInfo };
        
        try
        {
            process.Start();
            _logger.LogDebug("Process started with ID: {ProcessId}", process.Id);

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            _logger.LogDebug("Process output for {OperationType}: {Output}", operationType, output);
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Process error for {OperationType}: {Error}", operationType, error);
            }

            await process.WaitForExitAsync();
            stopwatch.Stop();

            bool success = process.ExitCode == 0 && string.IsNullOrEmpty(error);
            string result = success ? output : error;

            _logger.LogInformation(
                "Process {OperationType} completed in {ElapsedMs}ms. Success: {Success}, ExitCode: {ExitCode}",
                operationType, stopwatch.ElapsedMilliseconds, success, process.ExitCode);

            return new CodeExecutionResult(result, success, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, 
                "Process {OperationType} failed after {ElapsedMs}ms", 
                operationType, stopwatch.ElapsedMilliseconds);
            
            return new CodeExecutionResult(
                $"Process {operationType} failed: {ex.Message}", 
                false, 
                stopwatch.Elapsed);
        }
    }
}