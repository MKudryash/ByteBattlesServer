using System.Text;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class CCodeGenerator : ICodeGenerator
{
    public string GenerateExecutableCode(CodeSubmission submission)
    {
        // Генерация кода на C
        var sb = new StringBuilder();
            
        // Добавление библиотек
        sb.AppendLine("#include <stdio.h>");
        sb.AppendLine("#include <stdlib.h>");
        sb.AppendLine();
            
        // Основной код пользователя
        sb.AppendLine(submission.Code);
        sb.AppendLine();
            
        // Генерация main функции для тестирования
        sb.AppendLine("int main(int argc, char *argv[]) {");
        sb.AppendLine("    if (argc > 1) {");
        sb.AppendLine("        // Обработка аргументов командной строки");
        sb.AppendLine("    }");
        sb.AppendLine("    return 0;");
        sb.AppendLine("}");
            
        return sb.ToString();
    }

    public string GetFileExtension(ProgrammingLanguage language) => ".c";
}