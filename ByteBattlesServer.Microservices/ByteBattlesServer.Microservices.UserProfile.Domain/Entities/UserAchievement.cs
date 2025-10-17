namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class UserAchievement : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid AchievementId { get; private set; }
    public DateTime AchievedAt { get; private set; }

    public UserProfile UserProfile { get; private set; }
    public Achievement Achievement { get; private set; }

    private UserAchievement() { }

    public UserAchievement(Guid userProfileId, Guid achievementId)
    {
        UserProfileId = userProfileId;
        AchievementId = achievementId;
        AchievedAt = DateTime.UtcNow;
    }
}