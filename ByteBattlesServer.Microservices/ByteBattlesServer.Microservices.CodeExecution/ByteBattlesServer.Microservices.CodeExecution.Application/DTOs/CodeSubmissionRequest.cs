using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;

namespace ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;

    public class CodeSubmissionRequest
    {
        public string Code { get; set; }
        public ProgrammingLanguage Language { get; set; }
        public List<TestCaseDto> TestCases { get; set; }
    }
