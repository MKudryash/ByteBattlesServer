namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class LanguageInfoResponse
{
    public Guid LanguageId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortTitle { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string CompilerCommand { get; set; } = string.Empty;
    public string ExecutionCommand { get; set; } = string.Empty;
    public bool SupportsCompilation { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}