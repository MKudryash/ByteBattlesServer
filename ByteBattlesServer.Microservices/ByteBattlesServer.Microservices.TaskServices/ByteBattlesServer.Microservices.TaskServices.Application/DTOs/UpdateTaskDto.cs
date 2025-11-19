namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class UpdateTaskDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = null;
    public string? Description { get; set; }  = null;
    public string? Difficulty { get; set; }  = null;
    public string? Author { get; set; }  = null;
    public string? FunctionName { get; set; }  = null;
    public string? PatternMain { get; set; }  = null;
    public string? PatternFunction { get; set; }  = null;
    public string? Parameters { get; set; }  = null;
    public string? ReturnType { get; set; }  = null;
    public List<Guid>? LanguageIds { get; set; }  = null;
}