using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Services;

public class AchievementService
{
    private readonly List<Achievement> _availableAchievements;
    
    public AchievementService()
    {
        _availableAchievements = CreateDefaultAchievements();
    }
    
    private List<Achievement> CreateDefaultAchievements()
    {
        return new List<Achievement>
        {
            // === РЕШЕНИЕ ЗАДАЧ ===
            new Achievement(
                name: "Первая кровь",
                description: "Решите свою первую задачу",
                iconUrl: "/achievements/first-blood.png",
                type: AchievementType.ProblemsSolved,
                requiredValue: 1,
                rewardExperience: 100,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Common,
                unlockMessage: "Отличный старт! Первая задача решена!"
            ),
            
            new Achievement(
                name: "Решатель",
                description: "Решите 10 задач",
                iconUrl: "/achievements/solver.png",
                type: AchievementType.ProblemsSolved,
                requiredValue: 10,
                rewardExperience: 250,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Common
            ),
            
            new Achievement(
                name: "Мастер алгоритмов",
                description: "Решите 100 задач",
                iconUrl: "/achievements/algorithm-master.png",
                type: AchievementType.ProblemsSolved,
                requiredValue: 100,
                rewardExperience: 1000,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Rare
            ),
            
            new Achievement(
                name: "Легкий как пух",
                description: "Решите 25 легких задач",
                iconUrl: "/achievements/easy-as-breeze.png",
                type: AchievementType.EasyProblemsSolved,
                requiredValue: 25,
                rewardExperience: 500,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Common
            ),
            
            // === БАТТЛЫ ===
            new Achievement(
                name: "Первая победа",
                description: "Выиграйте первый баттл",
                iconUrl: "/achievements/first-victory.png",
                type: AchievementType.Wins,
                requiredValue: 1,
                rewardExperience: 200,
                category: AchievementCategory.Battles,
                rarity: AchievementRarity.Common
            ),
            
            new Achievement(
                name: "Непобедимый",
                description: "Выиграйте 10 баттлов подряд",
                iconUrl: "/achievements/invincible.png",
                type: AchievementType.CurrentStreak,
                requiredValue: 10,
                rewardExperience: 1500,
                category: AchievementCategory.Streaks,
                rarity: AchievementRarity.Epic
            ),
            
            new Achievement(
                name: "Ветеран баттлов",
                description: "Участвуйте в 50 баттлах",
                iconUrl: "/achievements/battle-veteran.png",
                type: AchievementType.TotalBattles,
                requiredValue: 50,
                rewardExperience: 1000,
                category: AchievementCategory.Battles,
                rarity: AchievementRarity.Rare
            ),
            
            // === СТАТИСТИКА ===
            new Achievement(
                name: "Стрелок",
                description: "Достигните 80% успешных отправок",
                iconUrl: "/achievements/sharp-shooter.png",
                type: AchievementType.SuccessRate,
                requiredValue: 80,
                rewardExperience: 800,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Uncommon
            ),
            
            new Achievement(
                name: "Скорость света",
                description: "Среднее время выполнения менее 30 секунд",
                iconUrl: "/achievements/speed-of-light.png",
                type: AchievementType.AverageExecutionTime,
                requiredValue: 30, // секунд
                rewardExperience: 1200,
                category: AchievementCategory.Time,
                rarity: AchievementRarity.Epic
            ),
            
            // === СЕКРЕТНЫЕ ДОСТИЖЕНИЯ ===
            new Achievement(
                name: "Ниндзя кода",
                description: "Решите задачу за 10 секунд",
                iconUrl: "/achievements/code-ninja.png",
                type: AchievementType.FastestSubmission,
                requiredValue: 10,
                rewardExperience: 2000,
                category: AchievementCategory.Special,
                rarity: AchievementRarity.Legendary,
                isSecret: true
            ),
            
            new Achievement(
                name: "Перфекционист",
                description: "С первого раза успешно решите 5 задач подряд",
                iconUrl: "/achievements/perfectionist.png",
                type: AchievementType.PerfectStreak,
                requiredValue: 5,
                rewardExperience: 1500,
                category: AchievementCategory.Streaks,
                rarity: AchievementRarity.Epic,
                isSecret: true
            )
        };
    }
    
    public List<Achievement> GetAvailableAchievements()
    {
        return _availableAchievements;
    }
    
    public List<Achievement> GetUserAchievements(List<Guid> unlockedAchievementIds)
    {
        return _availableAchievements
            .Where(a => unlockedAchievementIds.Contains(a.Id))
            .ToList();
    }
    
    public List<Achievement> GetLockedAchievements(List<Guid> unlockedAchievementIds)
    {
        return _availableAchievements
            .Where(a => !unlockedAchievementIds.Contains(a.Id) && !a.IsSecret)
            .ToList();
    }
}