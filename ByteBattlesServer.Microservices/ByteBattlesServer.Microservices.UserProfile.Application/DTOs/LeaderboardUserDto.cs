namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;


public record LeaderboardUserDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    public string? AvatarUrl { get; init; }
    public string? Country { get; init; }
    public int Position { get; init; }
    public int TotalExperience { get; init; }
    public int BattlesWon { get; init; }
    public int ProblemsSolved { get; init; }
    public string Level { get; set; } 
}