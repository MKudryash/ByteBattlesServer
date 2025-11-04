namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class LanguageInfo
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortTitle { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;
    public string CompilerCommand { get; set; } = string.Empty;
    public string ExecutionCommand { get; set; } = string.Empty;
    public bool SupportsCompilation { get; set; }
}