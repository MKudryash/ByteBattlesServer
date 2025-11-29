using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using ByteBattlesServer.Microservices.TaskServices.Application.Mapping;
using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class GetTaskRandomHandler:IRequestHandler<GetRandomTask, TaskDto?>
{
    private readonly ITaskRepository  _repository;

    public GetTaskRandomHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskDto?> Handle(GetRandomTask request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetRandomByDifficultyAsync(request.Difficulty);

        return TaskMapping.MapToDtoAllInfo(task);
    }
}