namespace ByteBattlesServer.Domain.Results;

public class ErrorRequest : Exception
{
    public ErrorRequest(string message) : base(message)
    {
    }
}