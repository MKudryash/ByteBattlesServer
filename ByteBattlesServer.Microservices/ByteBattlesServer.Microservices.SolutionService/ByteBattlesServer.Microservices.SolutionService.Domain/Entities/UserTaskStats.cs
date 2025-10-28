namespace ByteBattlesServer.Microservices.SolutionService.Domain.Entities;

public class UserTaskStats : Entity
{
    public Guid UserId { get; private set; }
    public Guid TaskId { get; private set; }
    public int TotalAttempts { get; private set; }
    public int SuccessfulAttempts { get; private set; }
    public int FailedAttempts { get; private set; }
    public TimeSpan BestExecutionTime { get; private set; }
    public DateTime? FirstSolvedAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public bool IsSolved => SuccessfulAttempts > 0;

    public double SuccessRate => TotalAttempts > 0 ? (double)SuccessfulAttempts / TotalAttempts * 100 : 0;

    private UserTaskStats() { }

    public UserTaskStats(Guid userId, Guid taskId)
    {
        UserId = userId;
        TaskId = taskId;
        BestExecutionTime = TimeSpan.MaxValue;
    }

    public void RecordAttempt(UserSolution solution)
    {
        TotalAttempts++;
        LastAttemptAt = DateTime.UtcNow;

        if (solution.IsCorrect)
        {
            SuccessfulAttempts++;
            
            if (solution.ExecutionTime < BestExecutionTime || BestExecutionTime == TimeSpan.MaxValue)
            {
                BestExecutionTime = solution.ExecutionTime;
            }

            if (FirstSolvedAt == null)
            {
                FirstSolvedAt = solution.CompletedAt;
            }
        }
        else
        {
            FailedAttempts++;
        }
    }
}