namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;


public class RecentActivityDto
{
    public string Type { get; set; } 
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }
    public int ExperienceGained { get; set; }
}
