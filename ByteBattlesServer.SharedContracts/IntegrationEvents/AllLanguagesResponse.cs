namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class AllLanguagesResponse
{
    public List<LanguageInfo> Languages { get; set; } = new();
    public string CorrelationId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}