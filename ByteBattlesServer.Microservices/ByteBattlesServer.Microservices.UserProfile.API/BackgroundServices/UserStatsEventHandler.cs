using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

public class UserStatsEventHandler : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserStatsEventHandler> _logger; // Исправьте тип логгера

    public UserStatsEventHandler(
        IMessageBus messageBus,
        IServiceProvider serviceProvider,
        ILogger<UserStatsEventHandler> logger) // Исправьте тип логгера
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🟣 [UserProfile] Starting UserStatsEventHandler...");

        try
        {
            _messageBus.Subscribe<UserStatsIntegrationEvent>(
                "user_stats-events",
                "user-profile-stats-service-queue",
                "user.stats.update",
                HandleUserStatsEvent);

            _logger.LogInformation("🟢 [UserProfile] Successfully subscribed to user.stats.update events");

            // Ждем отмены вместо немедленного завершения
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("UserStatsEventHandler was cancelled");
        }
    }

    private async Task HandleUserStatsEvent(UserStatsIntegrationEvent arg)
    {
        _logger.LogInformation("🟣 [UserProfile] Received user stats update for user: {UserId}, successful: {IsSuccessful}", 
            arg.UserId, arg.isSuccessful);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            var command = new UpdateUserStatsCommand(
                arg.UserId, 
                arg.isSuccessful, 
                arg.difficulty, 
                arg.executionTime, 
                arg.taskId);

            var result = await mediator.Send(command);

            _logger.LogInformation("🟢 [UserProfile] Successfully updated stats for user: {UserId}, total solved: {TotalSolved}", 
                arg.UserId, result.Stats.TotalProblemsSolved);

            // Публикация события о обновлении профиля
            var profileUpdatedEvent = new UserStatsUpdateIntegrationEvent
            {
                UserId = arg.UserId,
                TotalProblemsSolved = result.Stats.TotalProblemsSolved,
                TotalBattles = result.Stats.TotalBattles,
                Wins = result.Stats.Wins,
                Losses = result.Stats.Losses,
                Draws = result.Stats.Draws,
                CurrentStreak = result.Stats.CurrentStreak,
                MaxStreak = result.Stats.MaxStreak,
                TotalExperience = result.Stats.TotalExperience,
                WinRate = result.Stats.WinRate,
                ExperienceToNextLevel = result.Stats.ExperienceToNextLevel,
            };

            _messageBus.Publish(
                profileUpdatedEvent,
                "user_stats-events",
                "user.profile.stats.update");

            _logger.LogInformation("🟢 [UserProfile] Published profile updated event for user: {UserId}", arg.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 [UserProfile] Error processing user stats event for user: {UserId}", arg.UserId);
            // Не бросаем исключение, чтобы не падал весь сервис
        }
    }
}