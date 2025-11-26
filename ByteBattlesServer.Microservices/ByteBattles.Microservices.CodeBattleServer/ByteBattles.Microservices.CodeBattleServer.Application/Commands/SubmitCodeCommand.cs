using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record SubmitCodeCommand(Guid RoomId, Guid UserId, Guid TaskId, string Code) : IRequest<SubmitCodeResponse>;