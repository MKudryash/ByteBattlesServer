namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

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