using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface ICodeCompiler
{
    Task<CodeExecutionResult> CompileAsync(string filePath, LanguageInfo language);
    Task<CodeExecutionResult> ExecuteAsync(string filePath, LanguageInfo language, string arguments);
}