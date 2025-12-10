using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.DTOs;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.Microservices.UserProfile.Domain.Exceptions;
using ByteBattlesServer.Microservices.UserProfile.Domain.Interfaces;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;

public class UpdateUserStatsCommandHandler : IRequestHandler<UpdateUserStatsCommand, UserProfileDto>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserStatsCommandHandler> _logger;

    public UpdateUserStatsCommandHandler(
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserStatsCommandHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserProfileDto> Handle(UpdateUserStatsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user stats for UserId: {UserId}", request.UserId);
        _logger.LogInformation("Updating type: {activityType}",request.activityType);

        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId)
            ?? throw new UserProfileNotFoundException(request.UserId);

        var expGained = CalculateExperience(request.difficulty);

        // Обработка решения задачи
        if (request.activityType == ActivityType.ProblemSolved && request.isSuccessful == true)
        {
            await HandleProblemSolution(userProfile, request, expGained, cancellationToken);
        }
        else
        {
            userProfile.AddTotalSubmissions( );
            _userProfileRepository.Update(userProfile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        _logger.LogInformation("Received activity type: {ActivityType}, IsSuccessful: {IsSuccessful}", 
            request.activityType, request.isSuccessful);
        Console.WriteLine("ЗАШЕЛ В РЕДАКТИРОВАНИЕ");
        Console.WriteLine(request.activityType);
        Console.WriteLine(request.battleId.HasValue);
        Console.WriteLine(request.battleId);
        // Обработка битвы
        if (request.activityType == ActivityType.Battle)
        {
            Console.WriteLine("ЗАШЕЛ В БИТВУ");
            await HandleBattleResult(userProfile, request, expGained, cancellationToken);
        }

        // Обновление уровня пользователя
        userProfile.UpdateLevel();
        _userProfileRepository.Update(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User stats updated successfully for UserId: {UserId}", request.UserId);

        return MapToDto(userProfile);
    }

    private async Task HandleProblemSolution(
        Domain.Entities.UserProfile userProfile,
        UpdateUserStatsCommand request,
        int expGained,
        CancellationToken cancellationToken)
    {
        
        if (request.taskId.HasValue && !userProfile.Stats.HasSolvedTask(request.taskId.Value))
        {
            userProfile.UpdateProblemStats(
                request.isSuccessful ?? false,
                request.difficulty?? TaskDifficulty.Easy,
                request.executionTime?? TimeSpan.Zero,
                request.taskId.Value);

            _userProfileRepository.Update(userProfile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Добавляем активность только если это успешное решение
        if (request.isSuccessful == true)
        {
            var activityDescription = $"Сложность: {request.difficulty}, Язык: {request.language}";
            var activity = new RecentActivity(
                userProfile.Id,
                request.activityType?? ActivityType.ProblemSolved,
                $"Решена задача: {request.problemTitle ?? "Неизвестная задача"}",
                activityDescription,
                expGained);

            await _userProfileRepository.AddRecentActivityAsync(activity);

            // Добавляем в недавние задачи
            if (request.taskId.HasValue)
            {
                var recentProblem = new RecentProblem(
                    userProfile.Id,
                    request.taskId.Value,
                    request.problemTitle ?? "Неизвестная задача",
                    request.difficulty?? TaskDifficulty.Easy,
                    request.language);

                await _userProfileRepository.AddRecentProblemAsync(recentProblem);
            }
        }

        userProfile.UpdatedAt = DateTime.UtcNow;
    }

    private async Task HandleBattleResult(
        Domain.Entities.UserProfile userProfile,
        UpdateUserStatsCommand request,
        int expGained,
        CancellationToken cancellationToken)
    {
        if (!request.battleId.HasValue) return;

        var result = (request.isSuccessful == true) ? BattleResultType.Win : BattleResultType.Loss;
        var battleExp = (request.isSuccessful == true) ? expGained * 5 : 0;

        Console.WriteLine($"Battle exp: {battleExp}");
        var battle = new BattleResult(
            userProfile.Id,
            request.battleId.Value,
            request.battleOpponent ?? "Неизвестный соперник",
            result,
            battleExp,
            request.taskId ?? Guid.Empty,
            request.executionTime?? TimeSpan.Zero );

        await _userProfileRepository.AddBattleResultAsync(battle);

        // Добавляем активность
        var activityDescription = $"Результат: {result}, Опыт: {battleExp}";
        var activity = new RecentActivity(
            userProfile.Id,
            request.activityType?? ActivityType.Battle,
            $"Завершена битва",
            activityDescription,
            battleExp);

        await _userProfileRepository.AddRecentActivityAsync(activity);

        // Обновляем статистику
        userProfile.Stats.UpdateStats(battle);

        userProfile.UpdatedAt = DateTime.UtcNow;
    }

    private int CalculateExperience(TaskDifficulty? difficulty)
    {
        return difficulty switch
        {
            TaskDifficulty.Easy => 100,
            TaskDifficulty.Medium => 250,
            TaskDifficulty.Hard => 500,
            _ => 0
        };
    }

    private UserProfileDto MapToDto(Domain.Entities.UserProfile userProfile)
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
        };
    }
}