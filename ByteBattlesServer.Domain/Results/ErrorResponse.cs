namespace ByteBattlesServer.Domain.Results;

public class ErrorResponse
{
    public string Message { get; set; }
    public string Code { get; set; }

    public ErrorResponse(string message, string code )
    {
        Message = message;
        Code = code;
    }
}