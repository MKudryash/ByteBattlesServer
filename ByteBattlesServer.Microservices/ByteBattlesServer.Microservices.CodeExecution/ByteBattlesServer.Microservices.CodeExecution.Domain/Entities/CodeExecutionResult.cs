namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;

public class CodeExecutionResult
{
    public string Output { get; }
    public bool IsSuccess { get; }
    public TimeSpan ExecutionTime { get; }
        
    public CodeExecutionResult(string output, bool isSuccess, TimeSpan executionTime)
    {
        Output = output;
        IsSuccess = isSuccess;
        ExecutionTime = executionTime;
    }
}