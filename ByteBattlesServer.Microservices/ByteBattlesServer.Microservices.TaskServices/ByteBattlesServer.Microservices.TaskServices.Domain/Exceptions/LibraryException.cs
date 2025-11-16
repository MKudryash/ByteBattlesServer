namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class LibraryException : Exception
{
    public string ErrorCode { get; }

    public LibraryException(string message, string errorCode = "LIBRARY_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}