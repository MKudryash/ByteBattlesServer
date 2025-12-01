using ByteBattlesServer.Domain.enums;
using ByteBattlesServer.SharedContracts.IntegrationEvents;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;


public class RecentProblem : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid ProblemId { get; private set; }
    public string Title { get; private set; }
    public TaskDifficulty Difficulty { get; private set; }
    public DateTime SolvedAt { get; private set; }
    public string Language { get; private set; }

    public UserProfile UserProfile { get; private set; }

    private RecentProblem() { }

    public RecentProblem(Guid userProfileId, Guid problemId, string title, 
        TaskDifficulty difficulty, string language)
    {
        UserProfileId = userProfileId;
        ProblemId = problemId;
        Title = title;
        Difficulty = difficulty;
        Language = language;
        SolvedAt = DateTime.UtcNow;
    }
}