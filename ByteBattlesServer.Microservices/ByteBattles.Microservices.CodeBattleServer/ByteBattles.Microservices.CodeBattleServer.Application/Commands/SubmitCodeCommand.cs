using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record SubmitCodeCommand(Guid RoomId, Guid UserId, TaskInfo Task, string Code) : IRequest<SubmitCodeResponse>;