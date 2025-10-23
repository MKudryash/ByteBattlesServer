namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class CreateTestCaseDto
{
    public string Input { get; set; }
    public string Output { get; set; }
    public bool IsExample { get; set; }
}