namespace ByteBattlesServer.Microservices.CodeExecution.Domain.Entities;

public class TestResult
{
    public bool AllTestsPassed { get; }
    public IReadOnlyList<TestCaseResult> Results { get; }
    public string Summary { get; }
        
    public TestResult(bool allTestsPassed, IEnumerable<TestCaseResult> results, string summary)
    {
        AllTestsPassed = allTestsPassed;
        Results = results?.ToList() ?? throw new ArgumentNullException(nameof(results));
        Summary = summary;
    }
}