using ByteBattlesServer.Domain.Results;
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
        var existingProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (existingProfile != null)
            throw new UserProfileAlreadyExistsException(request.UserId);

        var profile = new Domain.Entities.UserProfile(request.UserId, request.UserName);
        await _userProfileRepository.AddAsync(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(profile);
    }
    private UserProfileDto MapToDto(Domain.Entities.UserProfile profile) => new()
    {
        Id = profile.Id,
        UserId = profile.UserId,
        UserName = profile.UserName,
        AvatarUrl = profile.AvatarUrl,
        Bio = profile.Bio,
        Country = profile.Country,
        GitHubUrl = profile.GitHubUrl,
        LinkedInUrl = profile.LinkedInUrl,
        Level = profile.Level.ToString(),
        Stats = new UserStatsDto
        {
            TotalProblemsSolved = profile.Stats.TotalProblemsSolved,
            TotalBattles = profile.Stats.TotalBattles,
            Wins = profile.Stats.Wins,
            Losses = profile.Stats.Losses,
            Draws = profile.Stats.Draws,
            CurrentStreak = profile.Stats.CurrentStreak,
            MaxStreak = profile.Stats.MaxStreak,
            TotalExperience = profile.Stats.TotalExperience,
            WinRate = profile.Stats.WinRate,
            ExperienceToNextLevel = UserLevelCalculator.GetExperienceForNextLevel(profile.Level, profile.Stats.TotalExperience)
        },
        Settings = new UserSettingsDto
        {
            EmailNotifications = profile.Settings.EmailNotifications,
            BattleInvitations = profile.Settings.BattleInvitations,
            AchievementNotifications = profile.Settings.AchievementNotifications,
            Theme = profile.Settings.Theme,
            CodeEditorTheme = profile.Settings.CodeEditorTheme,
            PreferredLanguage = profile.Settings.PreferredLanguage
        },
        IsPublic = profile.IsPublic,
        CreatedAt = profile.CreatedAt
    };
}