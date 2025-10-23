namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class TestCaseDto
{
    public Guid Id { get; set; }
    public string Input { get; set; }
    public string Output { get; set; }
    public bool? IsExample { get; set; }
}