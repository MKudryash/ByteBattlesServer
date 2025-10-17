using ByteBattlesServer.Microservices.UserProfile.Domain.Common;

namespace ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;

public interface IIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IIntegrationEvent evt);
}