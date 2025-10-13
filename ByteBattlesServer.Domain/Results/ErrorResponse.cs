namespace ByteBattlesServer.Domain.Results;

public class ErrorResponse
{
    public string Message { get; set; }
    public int Code { get; set; }

    public ErrorResponse(string message, int code )
    {
        Message = message;
        Code = code;
    }
}