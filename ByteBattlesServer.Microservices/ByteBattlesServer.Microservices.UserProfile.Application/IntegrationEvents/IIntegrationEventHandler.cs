using ByteBattlesServer.Microservices.UserProfile.Domain.Common;

namespace ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;


public interface IIntegrationEventHandler<in TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}