using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class UpdateTestsCaseCommandHandler:IRequestHandler<UpdateTestsCaseCommand, TestCaseDto>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTestsCaseCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TestCaseDto> Handle(UpdateTestsCaseCommand request, CancellationToken cancellationToken)
    {
        var testCase = await _repository.GetTestCaseByIdAsync(request.Id);
        if (testCase == null)
            throw new TestCaseNotFoundException(request.Id);
        
        testCase.Update(request.Input,request.Output, request.IsExample);
        
        _repository.UpdateTestCaseAsync(testCase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(testCase);
    }

    private TestCaseDto MapToDto(TestCases testCase) => new()
    {
        Id = testCase.Id,
        Input = testCase.Input,
        Output = testCase.ExpectedOutput,
        IsExample = testCase.IsExample,
    };
}