namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class Language:Entity
{

    public string? Title { get; private set; }

    public string? ShortTitle { get; private set; }
    
    public string FileExtension { get; set; } = string.Empty;
    public string CompilerCommand { get; set; } = string.Empty;
    public string ExecutionCommand { get; set; } = string.Empty;
    public bool SupportsCompilation { get; set; }
    public string PatternFunction { get; set; }
    public string PatternMain { get; set; }
    public DateTime CreatedAd { get; set; }
    
    public DateTime UpdatedAd { get; set; }
    
    public virtual ICollection<Library> Libraries { get; private set; } = new List<Library>();

    public virtual ICollection<TaskLanguage> TasksLanguage { get; private set; } = new List<TaskLanguage>();

    private Language() { }
    public Language(string title, string shortTitle,  string fileExtension, string compilerCommand, 
        string executionCommand, bool supportsCompilation, string patternFunction, string patternMain)
    {
        Title = title;
        ShortTitle = shortTitle;
        FileExtension = fileExtension;
        CompilerCommand = compilerCommand;
        ExecutionCommand = executionCommand;
        SupportsCompilation = supportsCompilation;
        PatternFunction = patternFunction;
        PatternMain = patternMain;
        CreatedAd = DateTime.UtcNow;
    }

    public void Update(string? title, string? shortTitle, string fileExtension, string compilerCommand, bool supportsCompilation,
       string executionCommand,  string patternFunction, string patternMain)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title =title.Trim();
        if (!string.IsNullOrWhiteSpace(shortTitle))
            ShortTitle =shortTitle.Trim();
        if (!string.IsNullOrWhiteSpace(fileExtension))
            FileExtension = fileExtension.Trim();
        if (!string.IsNullOrWhiteSpace(compilerCommand))
            CompilerCommand = compilerCommand.Trim();
        if(!string.IsNullOrWhiteSpace(executionCommand))
            ExecutionCommand = ExecutionCommand.Trim();
        if (!string.IsNullOrWhiteSpace(patternFunction))
            PatternFunction = patternFunction.Trim();
        if (!string.IsNullOrWhiteSpace(patternMain))
            PatternMain = patternMain.Trim();
        
        SupportsCompilation = supportsCompilation;
        UpdatedAd = DateTime.UtcNow;
    }

}