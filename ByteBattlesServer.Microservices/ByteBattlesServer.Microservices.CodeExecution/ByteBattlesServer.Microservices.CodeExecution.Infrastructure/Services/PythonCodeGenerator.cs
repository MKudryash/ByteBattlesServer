using System.Text;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class PythonCodeGenerator : ICodeGenerator
{
    public string GenerateExecutableCode(CodeSubmission submission, string? fileNameWithoutExtension = null)
    {
        var sb = new StringBuilder();
            
        foreach (var sL in submission.Libraries)
        {
            sb.Append($"import {sL.NameLibrary}");
        }
        sb.AppendLine();
        

        sb.AppendLine(submission.Code);
        sb.AppendLine();
        
        sb.AppendLine(submission.PatternMain);
        
        Console.WriteLine(sb.ToString());
        return sb.ToString();
    }

    public string GetFileExtension(ProgrammingLanguage language) => ".py";
}