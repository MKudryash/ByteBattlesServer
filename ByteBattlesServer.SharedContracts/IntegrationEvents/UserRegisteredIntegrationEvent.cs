namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class UserRegisteredIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

public class CodeSubmissionEvent
{
    public string Code { get; set; }
    public Guid Language { get; set; }
    public IReadOnlyList<TestCaseEvent> TestCases { get; set; }
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}

public class CodeSubmissionRequest
{
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}

public class CodeTestResultResponseEvent
{
    public bool AllTestsPassed { get; set; }
    public List<TestCaseEvent> Results { get; set; }
    public string Summary { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
    
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
}

public class TestCaseEvent
{
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string ActualOutput { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}
