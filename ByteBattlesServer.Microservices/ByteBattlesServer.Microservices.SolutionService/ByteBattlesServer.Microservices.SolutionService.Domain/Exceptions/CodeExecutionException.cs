namespace ByteBattlesServer.Microservices.SolutionService.Domain.Exceptions;

public class CodeExecutionException : Exception
{
    public string ErrorCode { get; }

    public CodeExecutionException(string message, string errorCode = "CODE_EXECUTION_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}