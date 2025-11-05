using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;


    public class CodeSubmission
    {
        public string Code { get; }
        public Guid Language { get; }
        public IReadOnlyList<TestCase> TestCases { get; }
        
        public CodeSubmission(string code, Guid language, IEnumerable<TestCase> testCases)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Language = language;
            TestCases = testCases?.ToList() ?? throw new ArgumentNullException(nameof(testCases));
        }
    }