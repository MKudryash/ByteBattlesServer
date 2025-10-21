namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class UpdateTaskDto
{
    public string? Title { get; set; } = null;
    public string? Description { get; set; }  = null;
    public string? Difficulty { get; set; }  = null;
    public string? Author { get; set; }  = null;
    public string? FunctionName { get; set; }  = null;
    public string? InputParameters { get; set; }  = null;
    public string? OutputParameters { get; set; }  = null;
    public List<Guid>? LanguageIds { get; set; }  = null;
}