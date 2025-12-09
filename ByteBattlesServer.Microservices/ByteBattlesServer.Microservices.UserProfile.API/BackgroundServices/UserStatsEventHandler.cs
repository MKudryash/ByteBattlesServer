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

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _messageBus.Subscribe<UserStatsIntegrationEvent>(
                    "user_stats-events",
                    "user-profile-stats-service-queue",
                    "user.stats.update",
                    HandleUserStatsEvent);

                _logger.LogInformation("🟢 [UserProfile] Successfully subscribed to user.stats.update events");
            
                // Ждем отмены вместо бесконечного цикла
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔴 [UserProfile] Failed to subscribe to RabbitMQ. Retrying in 10 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }

    private async Task HandleUserStatsEvent(UserStatsIntegrationEvent arg)
    {
        _logger.LogInformation("🟣 [UserProfile] Received user stats update for user: {UserId}, successful: {IsSuccessful}", 
            arg.UserId, arg.IsSuccessful);

        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        UpdateUserStatsCommand command;
        try
        {
            if (arg.IsSuccessful)
            {
                 command = new UpdateUserStatsCommand(
                    UserId: arg.UserId,
                    isSuccessful: true,
                    difficulty: arg.Difficulty,
                    executionTime: arg.ExecutionTime,
                    taskId: arg.TaskId,
                    problemTitle: arg.ProblemTitle,
                    language: arg.Language,
                    activityType: ActivityType.ProblemSolved, // Явно указываем тип активности
                    battleId: arg.BattleId,
                    battleOpponent: arg.BattleOponent,
                    totalSubmission: 1 // Учитываем одну успешную попытку
                );
            }
            else
            {
                 command = new UpdateUserStatsCommand(
                    UserId: arg.UserId,
                    isSuccessful: false,
                    difficulty: null,
                    executionTime: null,
                    taskId: arg.TaskId,
                    problemTitle: null,
                    language: null,
                    activityType: null,
                    battleId: null,
                    battleOpponent: null,
                    totalSubmission: 1 
                );
            }


            var result = await mediator.Send(command);

            _logger.LogInformation("🟢 [UserProfile] Successfully updated stats for user: {UserId}, total solved: {TotalSolved}", 
                arg.UserId, 0);

            // Публикация события о обновлении профиля
            var profileUpdatedEvent = new UserStatsUpdateIntegrationEvent
            {
                UserId = arg.UserId
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