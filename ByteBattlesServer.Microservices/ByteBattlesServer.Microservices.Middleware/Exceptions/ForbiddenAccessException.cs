namespace ByteBattlesServer.Microservices.Middleware.Exceptions;

public class ForbiddenAccessException : Exception
{
    public string ErrorCode { get; }
    
    public ForbiddenAccessException(string message) 
        : base(message)
    {
        ErrorCode = "FORBIDDEN";
    }
}