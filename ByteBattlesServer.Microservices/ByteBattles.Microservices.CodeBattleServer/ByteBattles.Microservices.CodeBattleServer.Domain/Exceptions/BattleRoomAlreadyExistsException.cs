namespace ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;

public class BattleRoomAlreadyExistsException:BattleRoomException
{
    public BattleRoomAlreadyExistsException(Guid battleRoomId) 
        : base($"Battle room already exists for battleRoomId ID: {battleRoomId}", "BATTLE_ROOM_ALREADY_EXISTS")
    {
    } 
    public BattleRoomAlreadyExistsException(string title) 
        : base($"BAttle already exists for task title: {title}", "BATTLE_ROOm_ALREADY_EXISTS")
    {
    }
}