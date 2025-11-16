namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class UpdateLanguageDto{
    public Guid Id { get; set; }
    public string Title { get; set; }
  public string ShortTitle { get; set; }
  public string FileExtension { get; set; } = string.Empty;
  public string CompilerCommand { get; set; } = string.Empty;
  public string ExecutionCommand { get; set; } = string.Empty;
  public bool SupportsCompilation { get; set; }
  public string Pattern { get; set; } = string.Empty;
  public List<LibraryDto> Libraries { get; set; } = new();
}