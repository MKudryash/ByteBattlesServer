using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record JoinRoomCommand(Guid RoomId, Guid UserId) : IRequest<ResponseSuccessJoin>;