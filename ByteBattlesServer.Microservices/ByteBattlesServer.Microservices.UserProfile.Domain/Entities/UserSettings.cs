namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class UserSettings : ValueObject
{
    public bool EmailNotifications { get; private set; } = true;
    public bool BattleInvitations { get; private set; } = true;
    public bool AchievementNotifications { get; private set; } = true;
    
    public string Theme { get; private set; } = "light";
    public string CodeEditorTheme { get; private set; } = "vs-light";
    public string PreferredLanguage { get; private set; } = "csharp";

    public void Update(
        string? preferredLanguage = null,
        string? theme = null,
        string? codeEditorTheme = null,
        bool? achievementNotifications = null,
        bool? battleInvitations = null,
        bool? emailNotifications = null)
    {
        if (!string.IsNullOrWhiteSpace(preferredLanguage))
            PreferredLanguage = preferredLanguage;
            
        if (!string.IsNullOrWhiteSpace(theme))
            Theme = theme;
            
        if (!string.IsNullOrWhiteSpace(codeEditorTheme))
            CodeEditorTheme = codeEditorTheme;
            
        if (achievementNotifications.HasValue)
            AchievementNotifications = achievementNotifications.Value;
            
        if (battleInvitations.HasValue)
            BattleInvitations = battleInvitations.Value;
            
        if (emailNotifications.HasValue)
            EmailNotifications = emailNotifications.Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return EmailNotifications;
        yield return BattleInvitations;
        yield return AchievementNotifications;
        yield return Theme;
        yield return CodeEditorTheme;
        yield return PreferredLanguage;
    }
}
