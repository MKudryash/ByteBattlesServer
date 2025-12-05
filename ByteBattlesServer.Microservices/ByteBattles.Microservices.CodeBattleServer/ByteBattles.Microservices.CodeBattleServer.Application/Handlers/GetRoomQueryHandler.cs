using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Application.Mapping;
using ByteBattles.Microservices.CodeBattleServer.Application.Queries;
using ByteBattles.Microservices.CodeBattleServer.Domain.Interfaces;
using MediatR;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Handlers;

public class GetRoomQueryHandler : IRequestHandler<GetRoomQuery, RoomDto>
{
    private readonly IBattleRoomRepository _battleRoomRepository;

    public GetRoomQueryHandler(IBattleRoomRepository battleRoomRepository)
    {
        _battleRoomRepository = battleRoomRepository;
    }


    public async Task<RoomDto> Handle(GetRoomQuery request, CancellationToken cancellationToken)
    {
        var room = await _battleRoomRepository.GetByIdAsync(request.roomId);

        return RoomMapping.MapToDto(room);
    }
}