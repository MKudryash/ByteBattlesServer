using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
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
        if (request.Role == UserRole.student)
        {
            var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);

            if (userProfile == null)
            {
                throw new UserProfileNotFoundException(request.UserId);
            }

            return MapToUserProfileDto(userProfile);
        }
        else
        {
            var userProfile = await _userProfileRepository.GetByUserIdTeacherAsync(request.UserId);

            if (userProfile == null)
            {
                throw new UserProfileNotFoundException(request.UserId);
            }

            return MapToUserProfileTeacherDto(userProfile);
        }
    }

    private static StudentProfileDto MapToUserProfileDto(Domain.Entities.UserProfile userProfile)
    {
        return new StudentProfileDto
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
    private static TeacherProfileDto MapToUserProfileTeacherDto(Domain.Entities.UserProfile userProfile)
    {
        return new TeacherProfileDto
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
            TeacherStats = new TeacherStatsDto
            {
                CreatedTasks = userProfile.TeacherStats.CreatedTasks,
                ActiveStudents = userProfile.TeacherStats.ActiveStudents,
                AverageRating = userProfile.TeacherStats.AverageRating,
                TotalSubmissions = userProfile.TeacherStats.TotalSubmissions
            }
        };
    }
}
public class TeacherStatsDto
{
    public int CreatedTasks { get; set; }
    public int ActiveStudents { get; set; }
    public double AverageRating { get; set; }
    public int TotalSubmissions { get; set; }
}
public class StudentProfileDto : UserProfileDto
{
    public UserLevel Level { get; set; }
    public UserStatsDto Stats { get; set; }
    public List<RecentProblemDto> RecentProblems { get; set; } = new List<RecentProblemDto>();
    public List<BattleResultDto> BattleHistory { get; set; } = new List<BattleResultDto>();
    public List<UserAchievementDto> Achievements { get; set; } =  new List<UserAchievementDto>();
}


public class TeacherProfileDto : UserProfileDto
{
    public TeacherStatsDto TeacherStats { get; set; }
}
