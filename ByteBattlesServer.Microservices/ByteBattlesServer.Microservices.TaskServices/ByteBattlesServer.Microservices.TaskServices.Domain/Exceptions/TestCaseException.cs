namespace ByteBattlesServer.Microservices.TaskServices.Domain.Exceptions;

public class TestCaseException : Exception
{
    public string ErrorCode { get; }

    public TestCaseException(string message, string errorCode = "TASK_CASE_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}