using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class BattleResult : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid BattleId { get; private set; }
    public string OpponentName { get; private set; }
    public BattleResultType Result { get; private set; }
    public int ExperienceGained { get; private set; }
    public int ProblemSolved { get; private set; }
    public TimeSpan CompletionTime { get; private set; }
    public DateTime BattleDate { get; private set; }

    public UserProfile UserProfile { get; private set; }

    private BattleResult() { }

    public BattleResult(Guid userProfileId, Guid battleId, string opponentName, 
        BattleResultType result, int experienceGained, int problemSolved, 
        TimeSpan completionTime)
    {
        UserProfileId = userProfileId;
        BattleId = battleId;
        OpponentName = opponentName;
        Result = result;
        ExperienceGained = experienceGained;
        ProblemSolved = problemSolved;
        CompletionTime = completionTime;
        BattleDate = DateTime.UtcNow;
    }
}