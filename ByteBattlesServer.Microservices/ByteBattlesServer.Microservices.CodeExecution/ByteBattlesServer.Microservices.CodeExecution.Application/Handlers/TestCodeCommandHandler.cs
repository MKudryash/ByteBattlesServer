
using ByteBattlesServer.Microservices.CodeExecution.Application.Commands;
using ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;
using ByteBattlesServer.Microservices.CodeExecution.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.CodeExecution.Application.Handlers;

public class TestCodeCommandHandler: IRequestHandler<TestCodeCommand, CodeTestResultResponse>
{
    private readonly ITestRunner _testRunner;

    public TestCodeCommandHandler(ITestRunner testRunner)
    {
        _testRunner = testRunner;
    }
    public async Task<CodeTestResultResponse> Handle(TestCodeCommand request, CancellationToken cancellationToken)
    {
        var submission = MapToDto(request);
        var testResult = await _testRunner.RunTestsAsync(submission);
        return MapToDtoCodeTestResult(testResult);
    }

    private CodeSubmission MapToDto(TestCodeCommand request) => new(request.Code, request.Language, request.TestCases.Select(x=>Map(x)));

    private TestCase Map(TestCaseDto dto) => new(dto.Input, dto.ExpectedOutput);

    private CodeTestResultResponse MapToDtoCodeTestResult(TestResult testResult) => new()
    {
        AllTestsPassed = testResult.AllTestsPassed,
        Results = testResult.Results.Select(x=>new TestCaseResultDto()
        {
            Input = x.TestCase.Input,
            ExpectedOutput = x.TestCase.ExpectedOutput,
            ActualOutput = x.ActualOutput,
            ExecutionTime = x.ExecutionTime,
            IsPassed = x.IsPassed,
            
        }).ToList(),
        Summary = testResult.Summary,
    };
}