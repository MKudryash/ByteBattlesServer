namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UserActivityResponse
{
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<RecentProblemDto> RecentProblems { get; set; } = new();
}