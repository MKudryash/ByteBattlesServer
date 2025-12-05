using ByteBattlesServer.Microservices.TaskServices.Application.Queries;
using ByteBattlesServer.Microservices.TaskServices.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Handlers;

public class GetTaskCountDiffictlyHandler: IRequestHandler<GetTaskCountDiffictly, TaskCountDifficatly>
{
    private readonly ITaskRepository _repository;

    public GetTaskCountDiffictlyHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskCountDifficatly> Handle(GetTaskCountDiffictly request, CancellationToken cancellationToken)
    {
        var result = await _repository.TaskCountDiffaclty();
        return new TaskCountDifficatly
        {
            Easy = result.Easy,
            Medium = result.Medium,
            Hard = result.Hard
        };
    }
}