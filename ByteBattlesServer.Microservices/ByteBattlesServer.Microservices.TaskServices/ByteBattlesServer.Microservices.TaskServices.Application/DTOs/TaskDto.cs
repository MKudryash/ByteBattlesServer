namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class TaskDto
{
  public Guid Id { get; set; }
  public  string Title { get; set; }
  public  string Description { get; set; }
  public  string Difficulty { get; set; }
  public  string Author { get; set; }
  public  string FunctionName { get; set; }
  public  string InputParameters { get; set; }
  public  string OutputParameters { get; set; }
  public  DateTime CreatedAt { get; set; }
  public  DateTime? UpdatedAt { get; set; }
  public  List<TaskLanguageDto> TaskLanguages { get; set; }
  public List<TestCaseDto> TestCases { get; set; }
}