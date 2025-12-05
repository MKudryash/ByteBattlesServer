namespace ByteBattlesServer.Microservices.UserProfile.Application.DTOs;

public class RecentProblemDto
{
    public Guid ProblemId { get; set; }
    public string Title { get; set; }
    public string Difficulty { get; set; } // "easy", "medium", "hard"
    public DateTime SolvedAt { get; set; }
    public string Language { get; set; }
}