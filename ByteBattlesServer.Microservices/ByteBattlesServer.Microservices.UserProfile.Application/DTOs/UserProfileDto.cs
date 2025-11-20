namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;


public class UserProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string Level { get; set; } = string.Empty;
    public UserSettingsDto Settings { get; set; } = new();
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
}
