using System.Text;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class CCodeGenerator : ICodeGenerator
{
    public string GenerateExecutableCode(CodeSubmission submission, string? fileNameWithoutExtension = null)
    {
        // Генерация кода на C
        var sb = new StringBuilder();
            
        // Добавление библиотек
        foreach (var sL in submission.Libraries)
        {
            sb.AppendLine($"#include <{sL.NameLibrary}>");
        }
        sb.AppendLine();
            
        // Основной код пользователя
        sb.AppendLine(submission.Code);
        sb.AppendLine();
            
        // Генерация main функции для тестирования
        
        sb.AppendLine(submission.PatternMain);
            
        Console.WriteLine(sb.ToString());
        
        return sb.ToString();
    }

    public string GetFileExtension(ProgrammingLanguage language) => ".c";
}