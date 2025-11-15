namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class Language:Entity
{

    public string? Title { get; private set; }

    public string? ShortTitle { get; private set; }
    
    public string FileExtension { get; set; } = string.Empty;
    public string CompilerCommand { get; set; } = string.Empty;
    public string ExecutionCommand { get; set; } = string.Empty;
    public bool SupportsCompilation { get; set; }

    public virtual ICollection<TaskLanguage> TasksLanguage { get; private set; } = new List<TaskLanguage>();

    private Language() { }
    public Language(string title, string shortTitle,  string fileExtension, string compilerCommand, string executionCommand, bool supportsCompilation)
    {
        Title = title;
        ShortTitle = shortTitle;
        FileExtension = fileExtension;
        CompilerCommand = compilerCommand;
        ExecutionCommand = executionCommand;
        SupportsCompilation = supportsCompilation;
    }

    public void Update(string? title, string? shortTitle)
    {
        if (!string.IsNullOrWhiteSpace(title))
            Title =title.Trim();
        if (!string.IsNullOrWhiteSpace(shortTitle))
            ShortTitle =shortTitle.Trim();
    }

}