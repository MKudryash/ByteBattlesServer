using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class UpdateUserStatsCommandHandler:IRequestHandler<UpdateUserStatsCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserSettingsCommandHandler> _logger;

    public UpdateUserStatsCommandHandler(
        IUserProfileRepository userProfileRepository, 
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserSettingsCommandHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<UserProfileDto> Handle(UpdateUserStatsCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.UserId);
        }
        if (userProfile.Stats == null)
        {
            userProfile.Stats = new UserStats();
        }
        
        userProfile.UpdateProblemStats(request.isSuccessful,request.difficulty,
            request.executionTime,request.taskId,request.problemTitle, request.language);
        
        userProfile.UpdatedAt = DateTime.UtcNow;
        _userProfileRepository.Update(userProfile);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
         return new UserProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            Bio = userProfile.Bio,
            Country = userProfile.Country,
            GitHubUrl = userProfile.GitHubUrl,
            LinkedInUrl = userProfile.LinkedInUrl,
            IsPublic = userProfile.IsPublic,
            CreatedAt = userProfile.CreatedAt,
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