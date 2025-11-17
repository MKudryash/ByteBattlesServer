namespace ByteBattlesServer.Microservices.Middleware.Exceptions;



public class UnauthorizedAccessException : Exception
{
    public string ErrorCode { get; }
    
    public UnauthorizedAccessException(string message) 
        : base(message)
    {
        ErrorCode = "UNAUTHORIZED";
    }
}