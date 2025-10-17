using ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;
using ByteBattlesServer.Microservices.UserProfile.Domain.Common;
using ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Services;

public class IntegrationEventService : IIntegrationEventService
{
    private readonly UserProfileDbContext _context;
    private readonly ILogger<IntegrationEventService> _logger;

    public IntegrationEventService(
        UserProfileDbContext context,
        ILogger<IntegrationEventService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddAndSaveEventAsync(IIntegrationEvent evt)
    {
        _logger.LogInformation("Enqueuing integration event {EventId} to be published later", evt.Id);
        
        // Здесь можно сохранить событие в Outbox таблицу для надежной доставки
        // Для простоты просто логируем
        await Task.CompletedTask;
    }

    public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
    { 
         // Здесь будет логика публикации событий через RabbitMQ/Kafka
        // Пока просто логируем
        _logger.LogInformation("Publishing integration events for transaction {TransactionId}", transactionId);
        await Task.CompletedTask;
    }
}