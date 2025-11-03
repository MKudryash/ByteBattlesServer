using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UserStatsCommandDto
{
    public bool isSuccessful { get; set; }
    public TaskDifficulty difficulty { get; set; }
    public TimeSpan executionTime { get; set; }
    public Guid taskId { get; set; }
}