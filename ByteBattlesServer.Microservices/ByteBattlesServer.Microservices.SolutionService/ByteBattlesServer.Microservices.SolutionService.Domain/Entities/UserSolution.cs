namespace ByteBattlesServer.Microservices.SolutionService.Domain.Entities;


public class UserSolution : Entity
{
    public Guid UserId { get; private set; }
    public Guid TaskId { get; private set; }
    public string Code { get; private set; }
    public string Language { get; private set; }
    public SolutionStatus Status { get; private set; }
    public bool IsCorrect { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? ExecutionResult { get; private set; }
    public TimeSpan ExecutionTime { get; private set; }
    public int AttemptNumber { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    

    private UserSolution() { }

    public UserSolution(Guid userId, Guid taskId, string code, string language, int attemptNumber)
    {
        UserId = userId;
        TaskId = taskId;
        Code = code;
        Language = language;
        AttemptNumber = attemptNumber;
        Status = SolutionStatus.Pending;
        SubmittedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessing()
    {
        Status = SolutionStatus.Processing;
    }

    public void MarkAsCompleted(bool isCorrect, string? executionResult, string? errorMessage, TimeSpan executionTime)
    {
        Status = isCorrect ? SolutionStatus.Success : SolutionStatus.Failed;
        IsCorrect = isCorrect;
        ExecutionResult = executionResult;
        ErrorMessage = errorMessage;
        ExecutionTime = executionTime;
        CompletedAt = DateTime.UtcNow;
    }

    public void MarkAsTimeout()
    {
        Status = SolutionStatus.Timeout;
        ErrorMessage = "Execution timeout";
        CompletedAt = DateTime.UtcNow;
    }
}