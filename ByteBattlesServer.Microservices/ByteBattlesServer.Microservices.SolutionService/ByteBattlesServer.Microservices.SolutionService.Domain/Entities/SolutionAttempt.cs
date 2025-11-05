using ByteBattlesServer.Microservices.SolutionService.Domain.Enums;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Entities;

public class SolutionAttempt : Entity
{
    public Guid SolutionId { get; private set; }
    public string Code { get; private set; }
    public DateTime AttemptedAt { get; private set; }
    public SolutionStatus Status { get; private set; }
    public TimeSpan? ExecutionTime { get; private set; }
    public int? MemoryUsed { get; private set; }

    private SolutionAttempt() { }

    public SolutionAttempt(Guid solutionId, string code)
    {
        SolutionId = solutionId;
        Code = code;
        AttemptedAt = DateTime.UtcNow;
        Status = SolutionStatus.Pending;
    }

    public void UpdateStatus(SolutionStatus status, TimeSpan? executionTime = null, int? memoryUsed = null)
    {
        Status = status;
        ExecutionTime = executionTime;
        MemoryUsed = memoryUsed;
    }
}