using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Commands;

public record UpdateUserSettingsCommand(
    Guid UserId,
    bool EmailNotifications,
    bool BattleInvitations,
    bool AchievementNotifications,
    string Theme,
    string CodeEditorTheme,
    string PreferredLanguage) : IRequest;