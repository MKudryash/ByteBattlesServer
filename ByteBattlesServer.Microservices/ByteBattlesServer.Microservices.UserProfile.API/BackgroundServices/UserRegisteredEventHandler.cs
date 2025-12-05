using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.SharedContracts.IntegrationEvents;
using ByteBattlesServer.SharedContracts.Messaging;

namespace ByteBattlesServer.Microservices.UserProfile.API.BackgroundServices;

using MediatR;

public class UserRegisteredEventHandler : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(
        IMessageBus messageBus,
        IServiceProvider serviceProvider,
        ILogger<UserRegisteredEventHandler> logger)
    {
        _messageBus = messageBus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🟣 [UserProfile] Starting UserRegisteredEventHandler...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _messageBus.Subscribe<UserRegisteredIntegrationEvent>(
                    "user-events",
                    "user-profile-service-queue",
                    "user.registered",
                    HandleUserRegisteredEvent);

                _logger.LogInformation("Subscribed to user.registered events");
            
                // Ждем отмены вместо бесконечного цикла
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
    private async Task HandleUserRegisteredEvent(UserRegisteredIntegrationEvent @event)
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        try
        {
            _logger.LogInformation("Processing user registered event for user: {UserId}", @event.UserId);

            // Создание профиля пользователя
            var userName = GenerateUserName(@event.FirstName, @event.LastName);
            var command = new CreateUserProfileCommand(@event.UserId, userName,@event.Email,@event.IsPublic,@event.Role);

            var result = await mediator.Send(command);

            _logger.LogInformation("User profile created successfully for user: {UserId}", @event.UserId);

            // Публикация события о создании профиля (опционально)
            var profileCreatedEvent = new UserProfileCreatedIntegrationEvent
            {
                UserId = @event.UserId,
                ProfileId = result.Id,
                CreatedAt = DateTime.UtcNow
            };

            _messageBus.Publish(
                profileCreatedEvent,
                "user-events",
                "user.profile.created");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing user registered event for user: {UserId}", @event.UserId);
            throw;
        }
    }

    private static string GenerateUserName(string firstName, string lastName)
    {
        var baseUserName = $"{firstName.ToLower()}.{lastName.ToLower()}";
        var randomSuffix = new Random().Next(1000, 9999);
        return $"{baseUserName}{randomSuffix}";
    }
}