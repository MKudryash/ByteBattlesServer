using ByteBattlesServer.Microservices.CodeExecution.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;

    public class CodeSubmissionRequest
    {
        public string Code { get; set; }
        public LanguageInfo Language { get; set; }
        public List<TestCaseDto> TestCases { get; set; }
    }
