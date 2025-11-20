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

        var profile = new Domain.Entities.UserProfile(request.UserId, request.UserName,request.isPublic);
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
        Settings = new UserSettingsDto
        {
            EmailNotifications = profile.Settings.EmailNotifications,
            BattleInvitations = profile.Settings.BattleInvitations,
            AchievementNotifications = profile.Settings.AchievementNotifications,
            Theme = profile.Settings.Theme,
            CodeEditorTheme = profile.Settings.CodeEditorTheme,
            PreferredLanguage = profile.Settings.PreferredLanguage
        },
        Stats = new UserStatsDto
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
        },
        IsPublic = profile.IsPublic,
        CreatedAt = profile.CreatedAt
    };
}