using ByteBattles.Microservices.CodeBattleServer.Domain.Enums;

namespace ByteBattles.Microservices.CodeBattleServer.Application.DTOs;

public class RoomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public RoomStatus Status { get; set; }
    public Guid LanguageId { get; set; }
}