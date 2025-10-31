using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

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
                // [ProgrammingLanguage.Python] = new PythonCodeGenerator(),
                // [ProgrammingLanguage.Java] = new JavaCodeGenerator(),
                // [ProgrammingLanguage.Cpp] = new CppCodeGenerator()
            };
        }

        public string GenerateExecutableCode(CodeSubmission submission)
        {
            return _generators[submission.Language].GenerateExecutableCode(submission);
        }

        public string GetFileExtension(ProgrammingLanguage language)
        {
            if (_generators.TryGetValue(language, out var generator))
            {
                return generator.GetFileExtension(language);
            }
            throw new NotSupportedException($"Language {language} is not supported");
        }
    }
