using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;

namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;

public interface ITestRunner
{
    Task<TestResult> RunTestsAsync(CodeSubmission submission);
}