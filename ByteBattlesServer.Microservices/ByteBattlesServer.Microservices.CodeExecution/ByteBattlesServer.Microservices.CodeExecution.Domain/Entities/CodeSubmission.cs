using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;


    public class CodeSubmission
    {
        public string Code { get; }
        public LanguageInfo Language { get; }
        public IReadOnlyList<LibraryInfo> Libraries { get; }
        public string? PatternFunction { get;  }
        public string? PatternMain { get;  }
        public IReadOnlyList<TestCase> TestCases { get; }
        
        public CodeSubmission(string code, LanguageInfo language, IEnumerable<TestCase> testCases, List<LibraryInfo> libraries,string ? patternFunction, string? patternMain)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Language = language;
            TestCases = testCases?.ToList() ?? throw new ArgumentNullException(nameof(testCases));
            Libraries = libraries?.ToList() ?? throw new ArgumentNullException(nameof(libraries));
            PatternFunction = patternFunction;
            PatternMain = patternMain;
        }
    }