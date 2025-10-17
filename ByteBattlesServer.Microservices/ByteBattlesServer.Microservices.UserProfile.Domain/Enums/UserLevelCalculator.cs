namespace ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

public static class UserLevelCalculator
{
    private static readonly Dictionary<UserLevel, int> LevelThresholds = new()
    {
        [UserLevel.Beginner] = 0,
        [UserLevel.Novice] = 1000,
        [UserLevel.Intermediate] = 3000,
        [UserLevel.Advanced] = 6000,
        [UserLevel.Expert] = 10000,
        [UserLevel.Master] = 15000,
        [UserLevel.GrandMaster] = 25000
    };

    public static UserLevel CalculateLevel(int experience)
    {
        return LevelThresholds
            .Where(threshold => experience >= threshold.Value)
            .OrderByDescending(threshold => threshold.Value)
            .FirstOrDefault().Key;
    }

    public static int GetExperienceForNextLevel(UserLevel currentLevel, int currentExperience)
    {
        var nextLevel = currentLevel + 1;
        if (LevelThresholds.ContainsKey(nextLevel))
        {
            return LevelThresholds[nextLevel] - currentExperience;
        }
        return 0;
    }
}