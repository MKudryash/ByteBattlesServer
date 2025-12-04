using System.Text;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class JavaCodeGenerator : ICodeGenerator
{
    public string GenerateExecutableCode(CodeSubmission submission, string? fileNameWithoutExtension)
    {
        var sb = new StringBuilder();
            
        foreach (var sL in submission.Libraries)
        {
            sb.Append($"import {sL.NameLibrary};");
        }
        sb.AppendLine();
        
        sb.AppendLine("public class Program {");
        sb.AppendLine(submission.Code);
        sb.AppendLine();
        
        sb.AppendLine(submission.PatternMain);

        sb.AppendLine("}");
            
        
        Console.WriteLine(sb.ToString());
        return sb.ToString();
    }

    public string GetFileExtension(ProgrammingLanguage language) => ".java";
}