using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class GetTaskByIdQueryHandler: IRequestHandler<GetTaskByIdQuery, TaskDto?>
{
    private readonly ITaskRepository _repository;
    public GetTaskByIdQueryHandler(ITaskRepository repository) => _repository = repository;
    
    public async Task<TaskDto?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId);
        if (task ==null) 
            throw new TaskNotFoundException(request.TaskId);
        return MapToDto(task);
    }
    private TaskDto MapToDto(Domain.Entities.Task task) => new()
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        Difficulty = task.Difficulty.ToString(),
        Author = task.Author,
        FunctionName = task.FunctionName,
        InputParameters = task.InputParameters,
        OutputParameters = task.OutputParameters,
        TaskLanguages = task.TaskLanguages.Select(tl => new TaskLanguageDto()
        {
            LanguageId = tl.Id,
            LanguageTitle = tl.Language.Title,
            LanguageShortTitle = tl.Language.ShortTitle,
        }).ToList()
    };
}