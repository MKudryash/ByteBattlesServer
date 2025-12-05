using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class SearchQueryHandler:IRequestHandler<SearchQueryParams,List<StudentProfileDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public SearchQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }
    public async Task<List<StudentProfileDto>> Handle(SearchQueryParams request, CancellationToken cancellationToken)
    {
        var searchUsers = await _userProfileRepository.SearchAsync(request.SearchTerm, request.Page, request.PageSize);
        return await Task.FromResult(searchUsers.Select((profile) => MapToUserProfileDto(profile)).ToList());
    }
    private static StudentProfileDto MapToUserProfileDto(Domain.Entities.UserProfile profile)
    {
       return new StudentProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            UserName = profile.UserName,
            Email = profile.Email,
            AvatarUrl = profile.AvatarUrl,
            Bio = profile.Bio,
            Country = profile.Country,
            GitHubUrl = profile.GitHubUrl,
            LinkedInUrl = profile.LinkedInUrl,
            Level = profile.Level.ToString(),
            IsPublic = profile.IsPublic,
            CreatedAt = profile.CreatedAt,
            Settings = new UserSettingsDto
            {
                Theme = profile.Settings?.Theme ?? "light",
                CodeEditorTheme = profile.Settings?.CodeEditorTheme ?? "vs-light",
                AchievementNotifications = profile.Settings?.AchievementNotifications ?? true,
                BattleInvitations = profile.Settings?.BattleInvitations ?? true,
                EmailNotifications = profile.Settings?.EmailNotifications ?? true,
                PreferredLanguage = profile.Settings?.PreferredLanguage ?? "csharp",
            },
            Stats = profile.Stats != null ? new UserStatsDto
            {
                SuccessRate = profile.Stats.SuccessRate,
                TotalExecutionTime = profile.Stats.TotalExecutionTime,
                SolvedTaskIds = profile.Stats.SolvedTaskIds,
                TotalSubmissions = profile.Stats.TotalSubmissions,
                SuccessfulSubmissions = profile.Stats.SuccessfulSubmissions,
                TotalBattles = profile.Stats.TotalBattles,
                TotalProblemsSolved = profile.Stats.TotalProblemsSolved,
                WinRate = profile.Stats.WinRate,
                Wins = profile.Stats.Wins,
                Losses = profile.Stats.Losses,
                Draws = profile.Stats.Draws,
                CurrentStreak = profile.Stats.CurrentStreak,
                MaxStreak = profile.Stats.MaxStreak,
                EasyProblemsSolved = profile.Stats.EasyProblemsSolved,
                HardProblemsSolved = profile.Stats.HardProblemsSolved,
                MediumProblemsSolved = profile.Stats.MediumProblemsSolved,
                AverageExecutionTime = profile.Stats.AverageExecutionTime,
                TotalExperience = profile.Stats.TotalExperience
            } : null,
            RecentProblems = new List<RecentProblemDto>(),
            BattleHistory = new List<BattleResultDto>()
        };
    }
}