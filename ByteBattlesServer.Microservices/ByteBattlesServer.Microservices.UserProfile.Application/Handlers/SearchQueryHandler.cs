using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class SearchQueryHandler:IRequestHandler<SearchQueryParams,List<UserProfileDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public SearchQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<List<UserProfileDto>> Handle(SearchQueryParams request, CancellationToken cancellationToken)
    {
        var searchUsers = await _userProfileRepository.SearchAsync(request.SearchTerm, request.Page, request.PageSize);
        return await Task.FromResult(searchUsers.Select((profile) => MapToUserProfileDto(profile)).ToList());
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