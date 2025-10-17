namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class UserProfileCreatedIntegrationEvent
{
    public Guid UserId { get; set; }
    public Guid ProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
}