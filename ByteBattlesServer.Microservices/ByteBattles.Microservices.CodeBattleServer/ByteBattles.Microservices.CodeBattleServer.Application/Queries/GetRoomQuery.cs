using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Queries;


public record GetRoomQuery(Guid roomId) : IRequest<RoomDto>;

