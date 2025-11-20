namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class RecentActivity : Entity
{
    public Guid UserProfileId { get; private set; }
    public ActivityType Type { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime Timestamp { get; private set; }
    public int ExperienceGained { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public UserProfile UserProfile { get; private set; }

    private RecentActivity() { }

    public RecentActivity(Guid userProfileId, ActivityType type, string title, 
        string description, int experienceGained = 0)
    {
        UserProfileId = userProfileId;
        Type = type;
        Title = title;
        Description = description;
        ExperienceGained = experienceGained;
        Timestamp = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
}

public enum ActivityType
{
    Battle = 0,
    ProblemSolved = 1,
    Achievement = 2
}