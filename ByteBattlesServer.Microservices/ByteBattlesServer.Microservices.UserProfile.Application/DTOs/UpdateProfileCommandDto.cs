namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UpdateProfileCommandDto
{
    public string UserName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Country { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public bool IsPublic { get; set; }
}