namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TestCaseInfoRequest
{
    public Guid TaskId { get; set; }
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}

