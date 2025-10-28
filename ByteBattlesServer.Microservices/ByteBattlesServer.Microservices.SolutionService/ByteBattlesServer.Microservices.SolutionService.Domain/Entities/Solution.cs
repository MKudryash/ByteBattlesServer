using ByteBattlesServer.Microservices.SolutionService.Domain.Enums;

namespace ByteBattlesServer.Microservices.SolutionService.Domain.Entities;

public class Solution : Entity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid LanguageId { get; private set; }
    public string Code { get; private set; }
    public SolutionStatus Status { get; private set; }
    public DateTime SubmittedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public TimeSpan? ExecutionTime { get; private set; }
    public int? MemoryUsed { get; private set; }
    public int PassedTests { get; private set; }
    public int TotalTests { get; private set; }
    public double SuccessRate => TotalTests > 0 ? (double)PassedTests / TotalTests * 100 : 0;
    
    private readonly List<TestResult> _testResults = new();
    public IReadOnlyCollection<TestResult> TestResults => _testResults.AsReadOnly();

    private readonly List<SolutionAttempt> _attempts = new();
    public IReadOnlyCollection<SolutionAttempt> Attempts => _attempts.AsReadOnly();

    private Solution() { }

    public Solution(Guid taskId, Guid userId, Guid languageId, string code)
    {
        TaskId = taskId;
        UserId = userId;
        LanguageId = languageId;
        Code = code;
        Status = SolutionStatus.Pending;
        SubmittedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(SolutionStatus status, int passedTests, int totalTests, 
        TimeSpan? executionTime = null, int? memoryUsed = null)
    {
        Status = status;
        PassedTests = passedTests;
        TotalTests = totalTests;
        ExecutionTime = executionTime;
        MemoryUsed = memoryUsed;
        
        if (status == SolutionStatus.Completed || status == SolutionStatus.Failed)
        {
            CompletedAt = DateTime.UtcNow;
        }
    }

    public void AddTestResult(TestResult testResult)
    {
        _testResults.Add(testResult);
    }

    public void AddAttempt(SolutionAttempt attempt)
    {
        _attempts.Add(attempt);
    }
}