namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;


public class UserProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string Level { get; set; } = string.Empty;
    public UserStatsDto Stats { get; set; } = new();
    public UserSettingsDto Settings { get; set; } = new();
    public bool IsPublic { get; set; }
    public List<UserAchievementDto> Achievements { get; set; } = new();
    public List<BattleResultDto> RecentBattles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
