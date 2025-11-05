namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class UpdateTestCaseDto
{
    public string? Input { get; set; }
    public string? Output { get; set; }
    public bool IsExample { get; set; }
}