namespace ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;

public class TaskException : Exception
{
    public string ErrorCode { get; }

    public TaskException(string message, string errorCode = "TASK_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}