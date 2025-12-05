namespace ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;

public class BattleRoomUserNotFoundException : BattleRoomException
{
    public BattleRoomUserNotFoundException(Guid userId) 
        : base($"User ID {userId} not in Battle room", "BATTLE_ROOM_ALREADY_EXISTS")
    {
    } 
}