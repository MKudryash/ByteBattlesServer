namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UserSettingsDto
{
    public bool EmailNotifications { get; set; }
    public bool BattleInvitations { get; set; }
    public bool AchievementNotifications { get; set; }
    public string Theme { get; set; } = string.Empty;
    public string CodeEditorTheme { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = string.Empty;
}