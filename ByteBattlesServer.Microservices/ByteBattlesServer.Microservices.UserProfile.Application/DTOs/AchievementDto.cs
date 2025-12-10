namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class AchievementDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string IconUrl { get; set; }
    public string Category { get; set; }
    public string Rarity { get; set; }
    public bool IsSecret { get; set; }
    public DateTime? UnlockedAt { get; set; }
    public int? Progress { get; set; }
}

public class UserAchievementsResponse
{
    public int TotalAchievements { get; set; }
    public int UnlockedCount { get; set; }
    public List<AchievementDto> Achievements { get; set; }
    public Dictionary<string, int> AchievementsByCategory { get; set; }
}