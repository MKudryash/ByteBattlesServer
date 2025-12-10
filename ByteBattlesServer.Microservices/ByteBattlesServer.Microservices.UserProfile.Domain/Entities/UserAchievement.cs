namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class UserAchievement : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid AchievementId { get; private set; }
    public DateTime? UnlockedAt { get; set; }
    public int Progress { get; set; }
    public bool IsUnlocked { get; set; }
    
    // Навигационные свойства
    public UserProfile UserProfile { get; private set; }
    public Achievement Achievement { get; private set; }

    private UserAchievement() { }

    public UserAchievement(Guid userProfileId, Guid achievementId)
    {
        UserProfileId = userProfileId;
        AchievementId = achievementId;
        Progress = 0;
        IsUnlocked = false;
        UnlockedAt = null;
    }
    
    public void UpdateProgress(int newProgress)
    {
        Progress = newProgress;
    }
    
    public void Unlock()
    {
        if (!IsUnlocked)
        {
            IsUnlocked = true;
            UnlockedAt = DateTime.UtcNow;
        }
    }
}