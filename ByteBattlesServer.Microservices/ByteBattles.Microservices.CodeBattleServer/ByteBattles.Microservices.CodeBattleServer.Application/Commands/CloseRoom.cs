using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Commands;

public record CloseRoom(Guid RoomId):IRequest<CloseResponses>;