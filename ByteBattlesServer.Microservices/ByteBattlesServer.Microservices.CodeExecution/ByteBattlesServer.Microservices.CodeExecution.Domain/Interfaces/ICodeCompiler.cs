using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface ICodeCompiler
{
    Task<CodeExecutionResult> CompileAsync(string filePath, ProgrammingLanguage language);
    Task<CodeExecutionResult> ExecuteAsync(string filePath, ProgrammingLanguage language, string arguments);
}