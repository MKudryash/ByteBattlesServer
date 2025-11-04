using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;
using MediatR;

namespace ByteBattlesServer.Microservices.UserProfile.API.BackgroundServices;

public class UserStatsEventHandler : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserStatsEventHandler(
        IMessageBus messageBus,
        IServiceProvider serviceProvider,
        ILogger<UserRegisteredEventHandler> logger)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageBus.Subscribe<UserStatsIntegrationEvent>(
            "user_stats-events",
            "user-profile-stats-service-queue",
            "user.stats.update",
            HandleUserStatsEvent);

        _logger.LogInformation("Subscribed to user.registered events");

        return Task.CompletedTask;
    }

    private async Task HandleUserStatsEvent(UserStatsIntegrationEvent arg)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            
            var command = new UpdateUserStatsCommand(arg.UserId,arg.isSuccessful,arg.difficulty,arg.executionTime,arg.taskId);

            var result = await mediator.Send(command);

            
            // Публикация события о создании профиля (опционально)
            var profileCreatedEvent = new UserStatsUpdateIntegrationEvent
            {
                UserId = arg.UserId,
                TotalProblemsSolved = result.Stats.TotalProblemsSolved,
                TotalBattles = result.Stats.TotalBattles,
                Wins = result.Stats.Wins,
                Losses = result.Stats.Losses,
                Draws = result.Stats.Draws,
                CurrentStreak = 
                    result.Stats.CurrentStreak,
                MaxStreak = result.Stats.MaxStreak,
                TotalExperience = result.Stats.TotalExperience,
                WinRate = result.Stats.WinRate,
                ExperienceToNextLevel = result.Stats.ExperienceToNextLevel,
            };

            _messageBus.Publish(
                profileCreatedEvent,
                "user_stats-events",
                "user.profile.stats.update");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user registered event for user: {UserId}",arg.UserId);
            throw;
        }
    }
}