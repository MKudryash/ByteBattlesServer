using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserProfileQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
 
        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.UserId);
        }
        
        return MapToUserProfileDto(userProfile);
    }

    private static UserProfileDto MapToUserProfileDto(Domain.Entities.UserProfile userProfile)
    {
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
            },
            Stats = new UserStatsDto
            {
                SuccessRate = userProfile.Stats.SuccessRate,
                TotalExecutionTime = userProfile.Stats.TotalExecutionTime,
                SolvedTaskIds = userProfile.Stats.SolvedTaskIds,
                TotalSubmissions = userProfile.Stats.TotalSubmissions,
                SuccessfulSubmissions = userProfile.Stats.SuccessfulSubmissions,
                TotalBattles = userProfile.Stats.TotalBattles,
                TotalProblemsSolved = userProfile.Stats.TotalProblemsSolved,
                WinRate = userProfile.Stats.WinRate,
                Wins = userProfile.Stats.Wins,
                Losses = userProfile.Stats.Losses,
                Draws = userProfile.Stats.Draws,
                CurrentStreak = userProfile.Stats.CurrentStreak,
                MaxStreak = userProfile.Stats.MaxStreak,
                EasyProblemsSolved = userProfile.Stats.EasyProblemsSolved,
                HardProblemsSolved = userProfile.Stats.HardProblemsSolved,
                MediumProblemsSolved = userProfile.Stats.MediumProblemsSolved,
                AverageExecutionTime = userProfile.Stats.AverageExecutionTime,
            }
        };
    }
}