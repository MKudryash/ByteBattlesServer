namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TestCaseInfoResponse
{
    public Guid TaskId { get; set; }
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}