using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record LeaveRoomCommand(Guid RoomId, Guid UserId) : IRequest<ResponseSuccessLeave>;