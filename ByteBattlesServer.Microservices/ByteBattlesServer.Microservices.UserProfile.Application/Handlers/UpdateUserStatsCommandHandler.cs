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
    private readonly IAchievementRepository _achievementRepository;

    public UpdateUserStatsCommandHandler(
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserStatsCommandHandler> logger,
        IAchievementRepository achievementRepository)
    {
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _achievementRepository = achievementRepository;
    }

    public async Task<UserProfileDto> Handle(UpdateUserStatsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating user stats for UserId: {UserId}", request.UserId);
        _logger.LogInformation("Updating type: {activityType}", request.activityType);

        var userProfile = await _userProfileRepository.GetByUserIdAsync(request.UserId)
            ?? throw new UserProfileNotFoundException(request.UserId);

        var expGained = CalculateExperience(request.difficulty);
        var achievementsUnlocked = new List<Achievement>();

        // Обработка решения задачи
        if (request.activityType == ActivityType.ProblemSolved && request.isSuccessful == true)
        {
            await HandleProblemSolution(userProfile, request, expGained, cancellationToken);
            
            // Проверяем достижения для решения задач
            var problemAchievements = await CheckAndAwardProblemAchievements(userProfile, request);
            achievementsUnlocked.AddRange(problemAchievements);
        }
        else if (request.isSuccessful == false)
        {
            userProfile.AddTotalSubmissions();
        }

        // Обработка битвы
        if (request.activityType == ActivityType.Battle)
        {
            await HandleBattleResult(userProfile, request, expGained, cancellationToken);
            
            // Проверяем достижения для битв
            var battleAchievements = await CheckAndAwardBattleAchievements(userProfile, request);
            achievementsUnlocked.AddRange(battleAchievements);
        }

        // Обновление уровня пользователя
        userProfile.UpdateLevel();
        
        // Проверяем общие достижения
        var generalAchievements = await CheckAndAwardGeneralAchievements(userProfile);
        achievementsUnlocked.AddRange(generalAchievements);
        
        _userProfileRepository.Update(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

      

        _logger.LogInformation("User stats updated successfully for UserId: {UserId}. Unlocked achievements: {Count}", 
            request.UserId, achievementsUnlocked.Count);

        return MapToDto(userProfile);
    }

    private async Task<List<Achievement>> CheckAndAwardProblemAchievements(
        Domain.Entities.UserProfile userProfile,
        UpdateUserStatsCommand request)
    {
        var unlockedAchievements = new List<Achievement>();
        var allAchievements = await _achievementRepository.GetAllAsync();

        foreach (var achievement in allAchievements)
        {
            // Пропускаем если достижение уже получено
            if (userProfile.HasAchievement(achievement.Id))
                continue;

            bool shouldUnlock = false;

            switch (achievement.Type)
            {
                case AchievementType.TotalProblemsSolved:
                    shouldUnlock = userProfile.Stats.TotalProblemsSolved >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.EasyProblemsSolved:
                    if (request.difficulty == TaskDifficulty.Easy)
                        shouldUnlock = userProfile.Stats.EasyProblemsSolved >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.MediumProblemsSolved:
                    if (request.difficulty == TaskDifficulty.Medium)
                        shouldUnlock = userProfile.Stats.MediumProblemsSolved >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.HardProblemsSolved:
                    if (request.difficulty == TaskDifficulty.Hard)
                        shouldUnlock = userProfile.Stats.HardProblemsSolved >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.SuccessRate:
                    shouldUnlock = userProfile.Stats.SuccessRate >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.AverageExecutionTime:
                    if (request.executionTime.HasValue)
                    {
                        var avgTime = userProfile.Stats.AverageExecutionTime.TotalSeconds;
                        shouldUnlock = avgTime <= achievement.RequiredValue;
                    }
                    break;
                    
                case AchievementType.FastestSubmission:
                    if (request.executionTime.HasValue && request.executionTime.Value.TotalSeconds <= achievement.RequiredValue)
                    {
                        shouldUnlock = true;
                    }
                    break;
                
            }

            if (shouldUnlock)
            {
                await AwardAchievementToUser(userProfile, achievement);
                unlockedAchievements.Add(achievement);
            }
        }

        return unlockedAchievements;
    }

    private async Task<List<Achievement>> CheckAndAwardBattleAchievements(
        Domain.Entities.UserProfile userProfile,
        UpdateUserStatsCommand request)
    {
        var unlockedAchievements = new List<Achievement>();
        var allAchievements = await _achievementRepository.GetAllAsync();

        foreach (var achievement in allAchievements)
        {
            if (userProfile.HasAchievement(achievement.Id))
                continue;

            bool shouldUnlock = false;

            switch (achievement.Type)
            {
                case AchievementType.Wins:
                    if (request.isSuccessful == true)
                        shouldUnlock = userProfile.Stats.Wins >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.TotalBattles:
                    shouldUnlock = userProfile.Stats.TotalBattles >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.CurrentStreak:
                    shouldUnlock = userProfile.Stats.CurrentStreak >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.MaxStreak:
                    shouldUnlock = userProfile.Stats.MaxStreak >= achievement.RequiredValue;
                    break;
            }

            if (shouldUnlock)
            {
                await AwardAchievementToUser(userProfile, achievement);
                unlockedAchievements.Add(achievement);
            }
        }

        return unlockedAchievements;
    }

    private async Task<List<Achievement>> CheckAndAwardGeneralAchievements(
        Domain.Entities.UserProfile userProfile)
    {
        var unlockedAchievements = new List<Achievement>();
        var allAchievements = await _achievementRepository.GetAllAsync();

        foreach (var achievement in allAchievements)
        {
            if (userProfile.HasAchievement(achievement.Id))
                continue;

            bool shouldUnlock = false;

            switch (achievement.Type)
            {
                case AchievementType.TotalExperience:
                    shouldUnlock = userProfile.Stats.TotalExperience >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.TotalSubmissions:
                    shouldUnlock = userProfile.Stats.TotalSubmissions >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.AccuracyRate:
                    shouldUnlock = userProfile.Stats.SuccessRate >= achievement.RequiredValue;
                    break;
                    
                case AchievementType.TimeSpentCoding:
                    if (userProfile.Stats.TotalExecutionTime.TotalHours >= achievement.RequiredValue)
                    {
                        shouldUnlock = true;
                    }
                    break;
            }

            if (shouldUnlock)
            {
                await AwardAchievementToUser(userProfile, achievement);
                unlockedAchievements.Add(achievement);
            }
        }

        return unlockedAchievements;
    }

    private async Task AwardAchievementToUser(Domain.Entities.UserProfile userProfile, Achievement achievement)
    {
        // Используем userProfile.Id, а не userProfile.UserId!
        var userAchievement = new UserAchievement(
            userProfileId: userProfile.Id, // ← ИСПРАВЛЕНО!
            achievementId: achievement.Id
        );
    
        // Разблокируем достижение
        userAchievement.Unlock();
    
        await _userProfileRepository.AddUserAchievementAsync(userAchievement);
    
        // Начисляем опыт за достижение
        userProfile.Stats.AddExperience(achievement.RewardExperience);
    
        // Создаем запись в активности
        var activity = new RecentActivity(
            userProfileId: userProfile.Id, // ← Тут тоже используем userProfile.Id
            type: ActivityType.AchievementUnlocked,
            title: $"Получено достижение: {achievement.Name}",
            description: achievement.Description,
            experienceGained: achievement.RewardExperience
        );
    
        await _userProfileRepository.AddRecentActivityAsync(activity);
    
        _logger.LogInformation("Achievement '{AchievementName}' awarded to user {UserId}", 
            achievement.Name, userProfile.UserId);
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
  