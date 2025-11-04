namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class LanguageInfoRequest
{
    public Guid LanguageId { get; set; }
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}