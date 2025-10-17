namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UserAchievementDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public DateTime AchievedAt { get; set; }
}