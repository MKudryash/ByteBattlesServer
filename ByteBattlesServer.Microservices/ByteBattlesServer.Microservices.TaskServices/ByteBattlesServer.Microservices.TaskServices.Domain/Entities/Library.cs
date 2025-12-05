using System.Data;

namespace ByteBattlesServer.Microservices.TaskServices.Domain.Entities;

public class Library:Entity
{
    public string NameLibrary { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    
    public Language Language { get; set; }
    
    public DateTime CreatedAd { get; set; }
    
    public DateTime UpdatedAd { get; set; }
    public Guid LanguageId { get; set; }
    public virtual ICollection<TaskLibrary> Libraries { get; private set; } = new List<TaskLibrary>();
    
    
    public Library(){}

    public Library(string name, string description, string version, Guid languageId)
    {
        NameLibrary = name;
        Description = description;
        Version = version;
        LanguageId = languageId;
        CreatedAd = DateTime.UtcNow;
    }

    public void Update(string name, string description, string version)
    {
        if(!string.IsNullOrWhiteSpace(version))
            NameLibrary = name;
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
        if(!string.IsNullOrWhiteSpace(version))
            Version = version;
        UpdatedAd = DateTime.UtcNow;
    }
}