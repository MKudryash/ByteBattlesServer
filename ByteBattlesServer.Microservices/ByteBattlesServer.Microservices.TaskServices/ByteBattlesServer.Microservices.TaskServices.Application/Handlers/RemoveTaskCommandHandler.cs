using ByteBattlesServer.Microservices.TaskServices.Application.Commands;
using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class RemoveTaskCommandHandler : IRequestHandler<RemoveTaskCommand, DeleteResponseDto>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveTaskCommandHandler(ITaskRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }


    public async Task<DeleteResponseDto> Handle(RemoveTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId);
        if (task == null)
            throw new TaskNotFoundException(request.TaskId);


        var taskLanguage = await _repository.GetTaskLanguagesAsync(task.Id);
        foreach (var language in taskLanguage)
        {
            _repository.RemoveTaskLanguage(language);
        }  
        
        var taskLibrary = await _repository.GetTaskLibraryAsync(task.Id);
        foreach (var library in taskLibrary)
        {
            _repository.RemoveTaskLibrary(library);
        }
        
        var testCases = await _repository.GetTestCasesAsync(request.TaskId);
        foreach (var testCase in testCases)
        {
           await _repository.RemoveTestCaseAsync(testCase);
        }

        await _repository.Delete(task);
       await _unitOfWork.SaveChangesAsync();

        return new DeleteResponseDto("Task deleted successfully");
    }
}