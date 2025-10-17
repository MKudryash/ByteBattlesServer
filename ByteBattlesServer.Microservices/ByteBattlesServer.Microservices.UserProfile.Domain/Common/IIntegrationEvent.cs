namespace ByteBattlesServer.Microservices.UserProfile.Domain.Common;


public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}