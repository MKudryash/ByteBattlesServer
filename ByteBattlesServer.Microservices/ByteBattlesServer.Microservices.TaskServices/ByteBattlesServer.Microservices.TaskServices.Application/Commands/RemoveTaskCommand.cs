using ByteBattlesServer.Microservices.TaskServices.Application.DTOs;
using MediatR;

namespace ByteBattlesServer.Microservices.TaskServices.Application.Commands;

public record RemoveTaskCommand(Guid TaskId):IRequest<DeleteResponseDto>;