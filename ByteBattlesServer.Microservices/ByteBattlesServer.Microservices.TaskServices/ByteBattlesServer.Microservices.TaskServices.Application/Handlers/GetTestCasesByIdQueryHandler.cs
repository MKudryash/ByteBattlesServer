using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class GetTestCasesByTaskQueryHandler:IRequestHandler<GetTestCasesByTaskQuery, List<TestCaseDto>>
{
    private readonly ITaskRepository _repository;
    public GetTestCasesByTaskQueryHandler(ITaskRepository repository) => _repository = repository;
    
    public async Task<List<TestCaseDto>> Handle(GetTestCasesByTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId);
        if(task == null)
            throw new TaskNotFoundException(request.TaskId);
        
        var testCases = await _repository.GetTestCasesAsync(request.TaskId);
        if (!testCases.Any())
            throw new TestCaseNotFoundException(request.TaskId);

        return testCases.Select(tc => MapToDto(tc)).ToList();
    }
    private TestCaseDto MapToDto(TestCases testCase) => new()
    {
        Id = testCase.Id,
        Input = testCase.Input,
        Output = testCase.ExpectedOutput,
        IsExample = testCase.IsExample,
    };
}