using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Application.Queries;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class GetUserProfileByIdQueryHandler : IRequestHandler<GetUserProfileByIdQuery, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserProfileByIdQueryHandler(IUserProfileRepository userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        // Находим профиль по ID профиля (не UserId)
        var userProfile = await _userProfileRepository.GetByIdAsync(request.ProfileId);
        
        if (userProfile == null)
        {
            throw new UserProfileNotFoundException(request.ProfileId);
        }
        

        // Маппим в соответствующий DTO в зависимости от роли
        return userProfile.Role switch
        {
            UserRole.student => MapToStudentProfileDto(userProfile),
            UserRole.teacher => MapToTeacherProfileDto(userProfile),
            _ => MapToBaseProfileDto(userProfile)
        };
    }

    private static StudentProfileDto MapToStudentProfileDto(Domain.Entities.UserProfile userProfile)
    {
        var dto = new StudentProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            Email = userProfile.Email,
            AvatarUrl = userProfile.AvatarUrl,
            Bio = userProfile.Bio,
            Country = userProfile.Country,
            GitHubUrl = userProfile.GitHubUrl,
            LinkedInUrl = userProfile.LinkedInUrl,
            Level = userProfile.Level,
            IsPublic = userProfile.IsPublic,
            CreatedAt = userProfile.CreatedAt,
            Settings = new UserSettingsDto
            {
                Theme = userProfile.Settings?.Theme ?? "light",
                CodeEditorTheme = userProfile.Settings?.CodeEditorTheme ?? "vs-light",
                AchievementNotifications = userProfile.Settings?.AchievementNotifications ?? true,
                BattleInvitations = userProfile.Settings?.BattleInvitations ?? true,
                EmailNotifications = userProfile.Settings?.EmailNotifications ?? true,
                PreferredLanguage = userProfile.Settings?.PreferredLanguage ?? "csharp",
            },
            Achievements = userProfile.Achievements.Select(x=>
                
                new UserAchievementDto()
                {
                    Name = x.Achievement.Name,
                    Description = x.Achievement.Description,
                    AchievedAt = x.AchievedAt,
                    IconUrl = x.Achievement.IconUrl,
                }).ToList()
        };

        // Добавляем статистику только если она есть
        if (userProfile.Stats != null)
        {
            dto.Stats = new UserStatsDto
            {
                SuccessRate = userProfile.Stats.SuccessRate,
                TotalExecutionTime = userProfile.Stats.TotalExecutionTime,
                SolvedTaskIds = userProfile.Stats.SolvedTaskIds ,
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
                TotalExperience = userProfile.Stats.TotalExperience
            };
        }

        // Добавляем недавние проблемы
        dto.RecentProblems = userProfile.RecentProblems?
            .Select(rp => new RecentProblemDto
            {
                ProblemId = rp.ProblemId,
                Title = rp.Title,
                Difficulty = rp.Difficulty.ToString(),
                Language = rp.Language,
                SolvedAt = rp.SolvedAt
            })
            .ToList() ?? new List<RecentProblemDto>();

        // Добавляем историю битв
        dto.BattleHistory = userProfile.BattleHistory?
            .Select(bh => new BattleResultDto
            {
                BattleDate = bh.BattleDate,
                OpponentName = bh.OpponentName,
                ProblemsSolved = bh.ProblemSolved,
                CompletionTime = bh.CompletionTime,
                ExperienceGained = bh.ExperienceGained
            })
            .ToList() ?? new List<BattleResultDto>();

        return dto;
    }

    private static TeacherProfileDto MapToTeacherProfileDto(Domain.Entities.UserProfile userProfile)
    {
        var dto = new TeacherProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            Email = userProfile.Email,
            AvatarUrl = userProfile.AvatarUrl,
            Bio = userProfile.Bio,
            Country = userProfile.Country,
            GitHubUrl = userProfile.GitHubUrl,
            LinkedInUrl = userProfile.LinkedInUrl,
            IsPublic = userProfile.IsPublic,
            CreatedAt = userProfile.CreatedAt,
            Settings = new UserSettingsDto
            {
                Theme = userProfile.Settings?.Theme ?? "light",
                CodeEditorTheme = userProfile.Settings?.CodeEditorTheme ?? "vs-light",
                AchievementNotifications = userProfile.Settings?.AchievementNotifications ?? true,
                BattleInvitations = userProfile.Settings?.BattleInvitations ?? true,
                EmailNotifications = userProfile.Settings?.EmailNotifications ?? true,
                PreferredLanguage = userProfile.Settings?.PreferredLanguage ?? "csharp",
            }
        };

        // Добавляем статистику учителя только если она есть
        if (userProfile.TeacherStats != null)
        {
            dto.TeacherStats = new TeacherStatsDto
            {
                CreatedTasks = userProfile.TeacherStats.CreatedTasks,
                ActiveStudents = userProfile.TeacherStats.ActiveStudents,
                AverageRating = userProfile.TeacherStats.AverageRating,
                TotalSubmissions = userProfile.TeacherStats.TotalSubmissions
            };
        }

        return dto;
    }

    private static UserProfileDto MapToBaseProfileDto(Domain.Entities.UserProfile userProfile)
    {
        return new UserProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            UserName = userProfile.UserName,
            Email = userProfile.Email,
            AvatarUrl = userProfile.AvatarUrl,
            Bio = userProfile.Bio,
            Country = userProfile.Country,
            GitHubUrl = userProfile.GitHubUrl,
            LinkedInUrl = userProfile.LinkedInUrl,
            IsPublic = userProfile.IsPublic,
            CreatedAt = userProfile.CreatedAt,
            Settings = new UserSettingsDto
            {
                Theme = userProfile.Settings?.Theme ?? "light",
                CodeEditorTheme = userProfile.Settings?.CodeEditorTheme ?? "vs-light",
                AchievementNotifications = userProfile.Settings?.AchievementNotifications ?? true,
                BattleInvitations = userProfile.Settings?.BattleInvitations ?? true,
                EmailNotifications = userProfile.Settings?.EmailNotifications ?? true,
                PreferredLanguage = userProfile.Settings?.PreferredLanguage ?? "csharp",
            }
        };
    }
}