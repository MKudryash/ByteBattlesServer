namespace ByteBattlesServer.Microservices.CodeExecution.Application.DTOs;

public class CodeTestResultResponse
{
    public bool AllTestsPassed { get; set; }
    public List<TestCaseResultDto> Results { get; set; }
    public string Summary { get; set; }
    public TimeSpan TotalExecutionTime { get; set; }
}