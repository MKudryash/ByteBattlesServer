using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;

    public class CodeSubmissionRequest
    {
        public string Code { get; set; }
        public LanguageInfo Language { get; set; }
        public List<TestCaseDto> TestCases { get; set; }
        public List<LibraryInfo> Libraries { get; set; }
        public string PatternFunction { get; set; }
        public string PatternMain { get; set; }
    }
