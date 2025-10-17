using ByteBattlesServer.Microservices.UserProfile.Domain.Common;

namespace ByteBattlesServer.Microservices.UserProfile.Application.IntegrationEvents;

public record UserRegisteredIntegrationEvent(
    Guid UserId, 
    string Email, 
    string FirstName, 
    string LastName) : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}
