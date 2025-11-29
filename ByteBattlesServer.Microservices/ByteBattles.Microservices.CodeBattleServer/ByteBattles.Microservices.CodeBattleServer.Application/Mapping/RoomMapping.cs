using ByteBattles.Microservices.CodeBattleServer.Application.DTOs;
using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;

namespace ByteBattles.Microservices.CodeBattleServer.Application.Mapping;

public class RoomMapping
{
    public static RoomDto MapToDto(BattleRoom room) => new()
    {
        Id = room.Id,
        Name = room.Name,
        Status = room.Status
    };
}