namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class TaskLanguageDto{
   public Guid LanguageId  { get; set; }
   public string LanguageTitle   { get; set; }
   public string LanguageShortTitle  { get; set; }
   public string c { get; set; } = string.Empty;
   public string CompilerCommand { get; set; } = string.Empty;
   public string ExecutionCommand { get; set; } = string.Empty;
   public bool SupportsCompilation { get; set; }
   
}