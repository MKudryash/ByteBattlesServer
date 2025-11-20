using ByteBattlesServer.Domain.enums;

namespace ByteBattlesServer.SharedContracts.IntegrationEvents;

public class UserRegisteredIntegrationEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.student;
    public bool IsPublic { get; set; } =true;
    public DateTime RegisteredAt { get; set; }
}