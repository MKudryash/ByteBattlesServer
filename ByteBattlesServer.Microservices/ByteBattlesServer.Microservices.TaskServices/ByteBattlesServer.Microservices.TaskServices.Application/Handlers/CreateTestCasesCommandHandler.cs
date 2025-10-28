using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class CreateTestCasesCommandHandler : IRequestHandler<CreateTestCasesCommand, List<TestCaseDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTestCasesCommandHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TestCaseDto>> Handle(CreateTestCasesCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId);
        if (task == null)
            throw new TaskNotFoundException(request.TaskId);

       
        var createdTestCases = new List<TestCases>();
        
        foreach (var testCaseDto in request.TestCases)
        {
            
            var testCase = new TestCases(
                request.TaskId,
                testCaseDto.Input,
                testCaseDto.Output,
                testCaseDto.IsExample
            );

            await _taskRepository.AddTestCaseAsync(testCase);
            createdTestCases.Add(testCase);
        }

        task.UpdateDate();
        await _taskRepository.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdTestCases.Select(tc => new TestCaseDto()
        {
            Id = tc.Id,
            Input = tc.Input,
            Output = tc.ExpectedOutput
        }).ToList();
    }
}