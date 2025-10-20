using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class UpdateUserSettingsCommandHandler : IRequestHandler<UpdateUserSettingsCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserSettingsCommandHandler> _logger;

    public UpdateUserSettingsCommandHandler(
        IUserProfileRepository userProfileRepository, 
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserSettingsCommandHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserProfileDto> Handle(UpdateUserSettingsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating settings for user {UserId}", request.UserId);

        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.UserId);
        }

        _logger.LogInformation("Current settings: {Settings}", 
            System.Text.Json.JsonSerializer.Serialize(userProfile.Settings));

        if (userProfile.Settings == null)
        {
            userProfile.Settings = new UserSettings();
            _logger.LogInformation("Created new settings object");
        }
        
        userProfile.UpdateSettings(
            request.PreferredLanguage,
            request.Theme,
            request.CodeEditorTheme,
            request.AchievementNotifications,
            request.BattleInvitations,
            request.EmailNotifications
        );

        _logger.LogInformation("Updated settings: {Settings}", 
            System.Text.Json.JsonSerializer.Serialize(userProfile.Settings));

        userProfile.UpdatedAt = DateTime.UtcNow;
        _userProfileRepository.Update(userProfile);

        var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Save changes result: {Result}", result);

        return new UserProfileDto
        {
            Id = userProfile.Id,
            Settings = new UserSettingsDto
            {
                Theme = userProfile.Settings.Theme,
                CodeEditorTheme = userProfile.Settings.CodeEditorTheme,
                AchievementNotifications = userProfile.Settings.AchievementNotifications,
                BattleInvitations = userProfile.Settings.BattleInvitations,
                EmailNotifications = userProfile.Settings.EmailNotifications,
                PreferredLanguage = userProfile.Settings.PreferredLanguage,
            }
        };
    }   
}