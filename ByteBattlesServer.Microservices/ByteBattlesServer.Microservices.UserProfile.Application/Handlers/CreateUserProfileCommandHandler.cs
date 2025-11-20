using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserProfileCommandHandler(IUserProfileRepository userProfileRepository, IUnitOfWork unitOfWork)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserProfileDto> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        // Проверяем существование профиля по userId и email
        var existingProfileByUserId = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (existingProfileByUserId != null)
            throw new UserProfileAlreadyExistsException(request.UserId);

        if (!string.IsNullOrEmpty(request.email))
        {
            var existingProfileByEmail = await _userProfileRepository.GetByEmailAsync(request.email);
            if (existingProfileByEmail != null)
                throw new UserProfileAlreadyExistsException(request.UserId);
        }

        // Создаем профиль
        var profile = new Domain.Entities.UserProfile(
            request.UserId, 
            request.UserName, 
            request.email, 
            request.role, 
            request.isPublic);

        await _userProfileRepository.AddAsync(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Маппим в соответствующий DTO в зависимости от роли
        return request.role switch
        {
            UserRole.student => MapToStudentProfileDto(profile),
            UserRole.teacher => MapToTeacherProfileDto(profile),
            _ => MapToBaseProfileDto(profile)
        };
    }

    private static StudentProfileDto MapToStudentProfileDto(Domain.Entities.UserProfile profile)
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
            Level = profile.Level,
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

    private static TeacherProfileDto MapToTeacherProfileDto(Domain.Entities.UserProfile profile)
    {
        return new TeacherProfileDto
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
            TeacherStats = profile.TeacherStats != null ? new TeacherStatsDto
            {
                CreatedTasks = profile.TeacherStats.CreatedTasks,
                ActiveStudents = profile.TeacherStats.ActiveStudents,
                AverageRating = profile.TeacherStats.AverageRating,
                TotalSubmissions = profile.TeacherStats.TotalSubmissions
            } : null
        };
    }

    private static UserProfileDto MapToBaseProfileDto(Domain.Entities.UserProfile profile)
    {
        return new UserProfileDto
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
            }
        };
    }
}