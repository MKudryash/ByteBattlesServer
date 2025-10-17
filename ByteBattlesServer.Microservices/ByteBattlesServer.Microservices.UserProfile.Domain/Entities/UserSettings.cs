namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class UserSettings : ValueObject
{
    public bool EmailNotifications { get; private set; } = true;
    public bool BattleInvitations { get; private set; } = true;
    public bool AchievementNotifications { get; private set; } = true;
    public string Theme { get; private set; } = "light";
    public string CodeEditorTheme { get; private set; } = "vs-light";
    public string PreferredLanguage { get; private set; } = "csharp";

    public void UpdateSettings(bool emailNotifications, bool battleInvitations, 
        bool achievementNotifications, string theme, string codeEditorTheme, 
        string preferredLanguage)
    {
        EmailNotifications = emailNotifications;
        BattleInvitations = battleInvitations;
        AchievementNotifications = achievementNotifications;
        Theme = theme;
        CodeEditorTheme = codeEditorTheme;
        PreferredLanguage = preferredLanguage;
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
