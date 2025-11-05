namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class AllLanguagesRequest
{
    public string ReplyToQueue { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}