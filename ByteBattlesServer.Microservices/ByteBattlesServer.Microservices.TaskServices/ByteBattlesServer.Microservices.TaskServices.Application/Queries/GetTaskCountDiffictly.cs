using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record GetTaskCountDiffictly(): IRequest<TaskCountDifficatly>;