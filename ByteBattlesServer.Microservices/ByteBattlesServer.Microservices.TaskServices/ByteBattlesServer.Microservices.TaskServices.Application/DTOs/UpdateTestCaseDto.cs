namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class UpdateTestCaseDto
{
    public Guid Id { get; set; }
    public string? Input { get; set; }
    public string? Output { get; set; }
}