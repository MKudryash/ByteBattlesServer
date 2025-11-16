namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class UpdateLibraryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}