using System.Diagnostics;
using System.Text;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class ProcessService : ICodeCompiler
{
    private readonly ILogger<ProcessService> _logger;

    public ProcessService(ILogger<ProcessService> logger)
    {
        _logger = logger;
    }

    public async Task<CodeExecutionResult> CompileAsync(string filePath, LanguageInfo language)
    {
        _logger.LogInformation("Starting compilation for {Language} file: {FilePath}", language, filePath);
        
        try
        {
            var startInfo = CreateCompileProcessInfo(filePath, language.ShortTitle);
            _logger.LogDebug("Compile command: {FileName} {Arguments}", startInfo.FileName, startInfo.Arguments);
            
            return await ExecuteProcessAsync(startInfo, "compilation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Compilation failed for {Language} file: {FilePath}", language, filePath);
            throw;
        }
    }

    public async Task<CodeExecutionResult> ExecuteAsync(string filePath, LanguageInfo language, string arguments)
    {
        _logger.LogInformation("Starting execution for {Language} file: {FilePath} with arguments: {Arguments}", 
            language, filePath, arguments);
        
        try
        {
            var startInfo = CreateExecuteProcessInfo(filePath, language.ShortTitle, arguments);
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

    private ProcessStartInfo CreateCompileProcessInfo(string filePath, string language)
    {
        var outputFileName = Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath));
        
        _logger.LogDebug("Generated output file name: {OutputFileName}", outputFileName);
        
        return language.ToLower() switch
        {
            "c" => new ProcessStartInfo
            {
                FileName = "gcc",
                Arguments = $"{filePath} -o {outputFileName} -Wall -Wextra -std=c99",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            "csharp" => new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build {filePath} -o {Path.GetDirectoryName(filePath)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            "cpp" => new ProcessStartInfo
            {
                FileName = "g++",
                Arguments = $"{filePath} -o {outputFileName} -Wall -Wextra -std=c++17",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            _ => throw new NotSupportedException($"Language {language} is not supported for compilation")
        };
    }

    private ProcessStartInfo CreateExecuteProcessInfo(string filePath, string language, string arguments)
    {
        var outputFileName = Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath));
        
        _logger.LogDebug("Execution - File: {FilePath}, Output: {OutputFileName}, Args: {Arguments}", 
            filePath, outputFileName, arguments);

        return language.ToLower() switch
        {
            "c" or "cpp" => new ProcessStartInfo
            {
                FileName = outputFileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            "csharp" or "cs" => new ProcessStartInfo
            {
                FileName = "dotnet-exec",
                Arguments = $"{outputFileName}.cs --args \"{arguments}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            "python" or "py" => new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"{filePath} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            "java" => new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"{filePath} {arguments}",
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

            // Используем StringBuilder для сбора вывода
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            // Асинхронно читаем вывод
            var outputTask = Task.Run(async () =>
            {
                while (!process.StandardOutput.EndOfStream)
                {
                    var line = await process.StandardOutput.ReadLineAsync();
                    if (line != null)
                    {
                        outputBuilder.AppendLine(line);
                        _logger.LogDebug("Process output: {Output}", line);
                    }
                }
            });

            var errorTask = Task.Run(async () =>
            {
                while (!process.StandardError.EndOfStream)
                {
                    var line = await process.StandardError.ReadLineAsync();
                    if (line != null)
                    {
                        errorBuilder.AppendLine(line);
                        _logger.LogWarning("Process error: {Error}", line);
                    }
                }
            });

            // Ждем завершения процесса и чтения вывода
            await process.WaitForExitAsync();
            await Task.WhenAll(outputTask, errorTask);

            stopwatch.Stop();

            string output = outputBuilder.ToString().Trim();
            string error = errorBuilder.ToString().Trim();
            
            bool success = process.ExitCode == 0 && string.IsNullOrEmpty(error);
            string result = success ? output : (string.IsNullOrEmpty(error) ? output : error);

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
