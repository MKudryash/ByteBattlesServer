namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Difficulty { get; set; }
    public string Author { get; set; }
    public string FunctionName { get; set; }
    public string PatternMain { get; set; }
    public string PatternFunction{ get; set; }
    
    public string Parameters { get; set; }
    public string ReturnType { get; set; }
    public List<Guid> LanguageId { get; set; }
    public List<Guid> LibraryId { get; set; }
}