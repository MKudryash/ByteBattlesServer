namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class BattleResultDto
{
    public string OpponentName { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public int ExperienceGained { get; set; }
    public int ProblemsSolved { get; set; }
    public TimeSpan CompletionTime { get; set; }
    public DateTime BattleDate { get; set; }
}