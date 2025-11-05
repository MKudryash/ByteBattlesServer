namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TestCasesInfoResponse
{
    public List<TestCaseInfo> TestCases { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}