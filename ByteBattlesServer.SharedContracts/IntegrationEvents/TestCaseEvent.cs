namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TestCaseEvent
{
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string ActualOutput { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    
}