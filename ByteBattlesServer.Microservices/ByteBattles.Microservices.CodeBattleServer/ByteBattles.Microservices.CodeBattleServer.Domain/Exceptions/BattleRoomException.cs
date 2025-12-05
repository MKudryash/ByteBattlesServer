namespace ByteBattles.Microservices.CodeBattleServer.Domain.Exceptions;

public class BattleRoomException : Exception
{
    public string ErrorCode { get; }

    public BattleRoomException(string message, string errorCode = "BATTLE_ROOM_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}