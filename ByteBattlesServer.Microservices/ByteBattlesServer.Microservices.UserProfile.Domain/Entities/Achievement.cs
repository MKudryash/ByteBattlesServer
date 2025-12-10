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
    // Новые поля для лучшей категоризации
    public AchievementCategory Category { get; private set; }
    public AchievementRarity Rarity { get; private set; }
    public string? UnlockMessage { get; private set; }
    private Achievement()
    {
        // Конструктор для EF Core
    }
    
    public Achievement(
        Guid id,
        string name, 
        string description, 
        string iconUrl, 
        AchievementType type, 
        int requiredValue, 
        int rewardExperience, 
        AchievementCategory category,
        AchievementRarity rarity,
        bool isSecret,
        string? unlockMessage)
    {
        Id = id;
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        Type = type;
        RequiredValue = requiredValue;
        RewardExperience = rewardExperience;
        Category = category;
        Rarity = rarity;
        IsSecret = isSecret;
        UnlockMessage = unlockMessage;
    }



    public Achievement(string name, string description, string iconUrl, 
        AchievementType type, int requiredValue, int rewardExperience, 
        AchievementCategory category = AchievementCategory.General,
        AchievementRarity rarity = AchievementRarity.Common,
        bool isSecret = false,
        string? unlockMessage = null)
    {
        Name = name;
        Description = description;
        IconUrl = iconUrl;
        Type = type;
        RequiredValue = requiredValue;
        RewardExperience = rewardExperience;
        Category = category;
        Rarity = rarity;
        IsSecret = isSecret;
        UnlockMessage = unlockMessage ?? $"Поздравляем! Вы получили достижение: {name}";
    }
}
public enum AchievementCategory
{
    General,        // Общие
    Battles,        // Баттлы
    Problems,       // Решение задач
    Streaks,        // Серии
    Time,           // Временные
    Social,         // Социальные
    Special         // Специальные
}

// Перечисление для редкости достижений
public enum AchievementRarity
{
    Common,         // Обычный
    Uncommon,       // Необычный
    Rare,           // Редкий
    Epic,           // Эпический
    Legendary       // Легендарный
}