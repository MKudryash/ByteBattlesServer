namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TestCaseInfo
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public bool Hidden { get; set; }
}