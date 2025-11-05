namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;

public class TestCaseResult
{
    public TestCase TestCase { get; set; }
    public string ActualOutput { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
        
    public TestCaseResult(TestCase testCase, string actualOutput, bool isPassed, TimeSpan executionTime)
    {
        TestCase = testCase;
        ActualOutput = actualOutput;
        IsPassed = isPassed;
        ExecutionTime = executionTime;
    }
}