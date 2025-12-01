using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UserStatsCommandDto
{
    public bool isSuccessful { get; set; }
    public TaskDifficulty difficulty { get; set; }
    public TimeSpan executionTime { get; set; }
    public Guid taskId { get; set; }
    public string problemTitle { get; set; }
    public string language { get; set; }
    public ActivityType activityType { get; set; }
}