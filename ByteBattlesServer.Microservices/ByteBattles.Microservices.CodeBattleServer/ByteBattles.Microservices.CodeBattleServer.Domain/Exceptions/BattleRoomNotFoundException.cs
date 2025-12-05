namespace ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;

public class BattleRoomNotFoundException:BattleRoomException

{
    public BattleRoomNotFoundException(Guid battleRoomId) 
        : base($"Battle room not found for battle room ID: {battleRoomId}", "BATTLE_ROOM_NOT_FOUND")
    {
    }
    public BattleRoomNotFoundException(string title) 
        : base($"Battle roo, not found for title: {title}", "BATTLE_ROOM_NOT_FOUND")
    {
    }
}