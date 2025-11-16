namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class LibraryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public Guid languageId { get; set; }
}