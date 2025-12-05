using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.CodeExecution.Infrastructure.Services;

public class CodeGeneratorFactory : ICodeGenerator
{
    private readonly Dictionary<ProgrammingLanguage, ICodeGenerator> _generators;

    public CodeGeneratorFactory()
    {
        _generators = new Dictionary<ProgrammingLanguage, ICodeGenerator>
        {
            [ProgrammingLanguage.C] = new CCodeGenerator(),
            [ProgrammingLanguage.CSharp] = new CSharpCodeGenerator(),
            [ProgrammingLanguage.Python] = new PythonCodeGenerator(),
            [ProgrammingLanguage.Java] = new JavaCodeGenerator(),
            // [ProgrammingLanguage.Cpp] = new CppCodeGenerator()
        };
    }

    public string GenerateExecutableCode(CodeSubmission submission, string? fileNameWithoutExtension = null)
    {
        var language = MapToProgrammingLanguage(submission.Language);

        if (!_generators.TryGetValue(language, out var generator))
        {
            throw new NotSupportedException($"Language {submission.Language.ShortTitle} is not supported");
        }

        return generator.GenerateExecutableCode(submission,  fileNameWithoutExtension);
    }

    private ProgrammingLanguage MapToProgrammingLanguage(LanguageInfo languageInfo)
    {
        // Assuming LanguageInfo has a property that can be mapped to ProgrammingLanguage
        // You might need to adjust this based on your actual LanguageInfo structure
        return languageInfo.ShortTitle switch
        {
            ("C" or "c") => ProgrammingLanguage.C,
            "C#" or "csharp" => ProgrammingLanguage.CSharp,
            "Python" or "py" or "Py" => ProgrammingLanguage.Python,
            "Java" or "java" => ProgrammingLanguage.Java,
            "C++" => ProgrammingLanguage.Cpp,
            _ => throw new NotSupportedException($"Language {languageInfo.ShortTitle} is not supported")
        };
    }
}