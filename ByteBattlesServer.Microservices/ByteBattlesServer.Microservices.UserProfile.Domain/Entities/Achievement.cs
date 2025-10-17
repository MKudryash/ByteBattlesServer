using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Domain.Entities;

public class Achievement : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string IconUrl { get; private set; }
    public AchievementType Type { get; private set; }
    public int RequiredValue { get; private set; }
    public int RewardExperience { get; private set; }
    public bool IsSecret { get; private set; }

    private Achievement() { }

    public Achievement(string name, string description, string iconUrl, 
        AchievementType type, int requiredValue, int rewardExperience, bool isSecret = false)
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        Type = type;
        RequiredValue = requiredValue;
        RewardExperience = rewardExperience;
        IsSecret = isSecret;
    }
}