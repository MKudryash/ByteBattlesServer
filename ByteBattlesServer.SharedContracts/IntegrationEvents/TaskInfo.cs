using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.TaskServices.Domain.Enums;

namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class TaskInfo
{
    public Guid Id { get; set; }
    public string? Title { get; set; }

    public string? Description { get; set; }
    
    public TaskDifficulty Difficulty { get; set; }

    public string? Author { get; set; }

    public string? FunctionName { get; set; }

    public string? PatternFunction { get; set; }

    public string? PatternMain { get; set; }
    
    public string? Parameters { get; set; }
    public string? ReturnType { get; set; }
    public LanguageInfo? Language { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<TestCaseInfo> TestCases { get; set; } = new List<TestCaseInfo>();
    public virtual ICollection<LibraryInfo> Libraries { get; set; } = new List<LibraryInfo>();
}