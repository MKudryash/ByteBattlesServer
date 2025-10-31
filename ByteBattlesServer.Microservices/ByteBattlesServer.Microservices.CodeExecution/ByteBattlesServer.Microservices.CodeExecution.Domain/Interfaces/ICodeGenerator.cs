using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface ICodeGenerator
{
    string GenerateExecutableCode(CodeSubmission submission);
    string GetFileExtension();
}