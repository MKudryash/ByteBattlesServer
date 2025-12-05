namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class CodeSubmissionEvent
{
    public string Code { get; set; }
    public LanguageInfo Language { get; set; }
    public IReadOnlyList<TestCaseEvent> TestCases { get; set; }
    public List<LibraryInfo> Libraries { get; set; }
    public string PatternFunction { get; set; }
    public string PatternMain { get; set; }
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string? ReplyToRoutingKey { get; set; }
}