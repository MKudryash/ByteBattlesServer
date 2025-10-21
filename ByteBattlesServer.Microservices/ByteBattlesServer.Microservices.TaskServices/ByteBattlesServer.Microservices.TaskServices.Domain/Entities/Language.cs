namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class Language:Entity
{

    public string? Title { get; private set; }

    public string? ShortTitle { get; private set; }

    public virtual ICollection<TaskLanguage> TasksLanguage { get; private set; } = new List<TaskLanguage>();

    public Language(string title, string shortTitle)
    {
        Title = title;
        ShortTitle = shortTitle;
    }
}