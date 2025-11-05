namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class CodeSubmissionEvent
{
    public string Code { get; set; }
    public Guid Language { get; set; }
    public IReadOnlyList<TestCaseEvent> TestCases { get; set; }
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}