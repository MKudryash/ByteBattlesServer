using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record GetTaskCountDiffictly(): IRequest<TaskCountDifficatly>;
public record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskDto>;

public record TaskCountDifficatly()
{
    public int Easy { get; init; }
    public int Medium { get; init; }
    public int Hard { get; init; }
}