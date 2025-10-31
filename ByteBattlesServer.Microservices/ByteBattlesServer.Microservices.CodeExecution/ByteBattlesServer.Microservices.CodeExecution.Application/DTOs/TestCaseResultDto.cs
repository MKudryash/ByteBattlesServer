namespace ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;

public class TestCaseResultDto
{
    public string Input { get; set; }
    public string ExpectedOutput { get; set; }
    public string ActualOutput { get; set; }
    public bool IsPassed { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}