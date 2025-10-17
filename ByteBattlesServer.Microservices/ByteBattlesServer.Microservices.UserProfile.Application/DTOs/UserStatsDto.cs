namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class UserStatsDto
{
    public int TotalProblemsSolved { get; set; }
    public int TotalBattles { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
    public int CurrentStreak { get; set; }
    public int MaxStreak { get; set; }
    public int TotalExperience { get; set; }
    public double WinRate { get; set; }
    public int ExperienceToNextLevel { get; set; }
}