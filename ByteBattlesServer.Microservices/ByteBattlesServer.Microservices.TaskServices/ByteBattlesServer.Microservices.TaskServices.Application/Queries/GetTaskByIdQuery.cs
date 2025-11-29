using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Queries;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<TaskDto>;