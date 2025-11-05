using ByteBattlesServer.Microservices.SolutionService.Domain.Enums;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Entities;

public class TestResult : Entity
{
    public Guid SolutionId { get; private set; }
    public Guid TestCaseId { get; private set; }
    public TestStatus Status { get; private set; }
    public string? Input { get; private set; }
    public string? ExpectedOutput { get; private set; }
    public string? ActualOutput { get; private set; }
    public string? ErrorMessage { get; private set; }
    public TimeSpan ExecutionTime { get; private set; }
    public int MemoryUsed { get; private set; }
    public DateTime ExecutedAt { get; private set; }

    private TestResult() { }

    public TestResult(Guid solutionId, Guid testCaseId, string? input, string? expectedOutput)
    {
        SolutionId = solutionId;
        TestCaseId = testCaseId;
        Input = input;
        ExpectedOutput = expectedOutput;
        Status = TestStatus.Pending;
        ExecutedAt = DateTime.UtcNow;
    }

    public void UpdateResult(TestStatus status, string? actualOutput = null, 
        string? errorMessage = null, TimeSpan executionTime = default, int memoryUsed = 0)
    {
        Status = status;
        ActualOutput = actualOutput;
        ErrorMessage = errorMessage;
        ExecutionTime = executionTime;
        MemoryUsed = memoryUsed;
    }
}