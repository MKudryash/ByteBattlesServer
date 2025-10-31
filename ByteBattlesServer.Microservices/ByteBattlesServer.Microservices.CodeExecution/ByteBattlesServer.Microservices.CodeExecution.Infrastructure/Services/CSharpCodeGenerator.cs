using System.Text;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class CSharpCodeGenerator : ICodeGenerator
{
    public string GenerateExecutableCode(CodeSubmission submission)
    {
        var sb = new StringBuilder();
            
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine("public class Program {");
        sb.AppendLine(submission.Code);
        sb.AppendLine();
        sb.AppendLine("    public static void Main(string[] args) {");
        sb.AppendLine("        if (args.Length > 0) {");
        sb.AppendLine("            // Обработка аргументов");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine("}");
            
        return sb.ToString();
    }

    public string GetFileExtension(ProgrammingLanguage language) => ".cs";
}