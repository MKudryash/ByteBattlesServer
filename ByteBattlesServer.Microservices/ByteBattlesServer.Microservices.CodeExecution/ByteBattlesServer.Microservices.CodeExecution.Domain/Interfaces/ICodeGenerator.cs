using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface ICodeGenerator
{
    string GenerateExecutableCode(CodeSubmission submission, string? fileNameWithoutExtension = null);
   // string GetFileExtension(ProgrammingLanguage language);
}