using System.Diagnostics;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class ProcessService : ICodeCompiler
{
    public async Task<CodeExecutionResult> CompileAsync(string filePath, ProgrammingLanguage language)
    {
        var startInfo = CreateCompileProcessInfo(filePath, language);
        return await ExecuteProcessAsync(startInfo);
    }

    public async Task<CodeExecutionResult> ExecuteAsync(string filePath, ProgrammingLanguage language, string arguments)
    {
        var startInfo = CreateExecuteProcessInfo(filePath, language, arguments);
        return await ExecuteProcessAsync(startInfo);
    }

    private ProcessStartInfo CreateCompileProcessInfo(string filePath, ProgrammingLanguage language)
    {
        var  outputFileName= Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath));
        
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
                Arguments = $"build {filePath}",
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
        var  outputFileName= Path.Combine(Path.GetDirectoryName(filePath)!, Path.GetFileNameWithoutExtension(filePath));

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
            ProgrammingLanguage.CSharp => new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project {filePath} -- {arguments}",
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
                Arguments = $"{filePath} {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            _ => throw new NotSupportedException($"Language {language} is not supported for execution")
        };
    }

    private async Task<CodeExecutionResult> ExecuteProcessAsync(ProcessStartInfo startInfo)
    {
        var stopwatch = Stopwatch.StartNew();
            
        using var process = new Process { StartInfo = startInfo };
        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        string error =  await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
        stopwatch.Stop();

        bool success = process.ExitCode == 0 && string.IsNullOrEmpty(error);
        string result = success ? output : error;

        return new CodeExecutionResult(result, success, stopwatch.Elapsed);
    }
}