namespace ByteBattlesServer.Microservices.TaskServices.Application.DTOs;

public class CreateLanguageDto{
   public string Title { get; set; }
   public string ShortTitle  { get; set; }

   public string FileExtension { get; set; } 
   public string CompilerCommand { get; set; } 
   public string ExecutionCommand { get; set; }
   public bool SupportsCompilation { get; set; }
   public string Pattern { get; set; }
   
   public List<CreateLibraryDto>  Libraries { get; set; }
}