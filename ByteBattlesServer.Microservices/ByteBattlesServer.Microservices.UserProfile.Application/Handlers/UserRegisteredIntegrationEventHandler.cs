using ByteBattlesServer.Microservices.UserProfile.Application.Commands;
using ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.UserProfile.Application.Handlers;


public class UserRegisteredIntegrationEventHandler 
    : IIntegrationEventHandler<UserRegisteredIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredIntegrationEventHandler> _logger;

    public UserRegisteredIntegrationEventHandler(
        IMediator mediator,
        ILogger<UserRegisteredIntegrationEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredIntegrationEvent @event)
    {
        try
        {
            _logger.LogInformation("Handling UserRegistered integration event for user {UserId}", @event.UserId);
            
            // Генерируем username на основе имени и фамилии
            var userName = GenerateUserName(@event.FirstName, @event.LastName);
            
            var command = new CreateUserProfileCommand(@event.UserId, userName);
            var result = await _mediator.Send(command);

            if (result.Id==null)
            {
                _logger.LogInformation("Successfully created user profile for {UserId}", @event.UserId);
            }
            else
            {
                _logger.LogWarning("Failed to create user profile for {UserId}", 
                    @event.UserId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling UserRegistered integration event for user {UserId}", 
                @event.UserId);
            throw;
        }
    }

    private static string GenerateUserName(string firstName, string lastName)
    {
        var baseUserName = $"{firstName}{lastName}".ToLowerInvariant();
        
        // Убираем не-ASCII символы и пробелы
        var userName = new string(baseUserName
            .Where(c => char.IsLetterOrDigit(c))
            .ToArray());

        // Если имя пользователя пустое, генерируем случайное
        if (string.IsNullOrEmpty(userName))
        {
            userName = $"user{DateTime.UtcNow.Ticks % 1000000}";
        }

        return userName;
    }
}