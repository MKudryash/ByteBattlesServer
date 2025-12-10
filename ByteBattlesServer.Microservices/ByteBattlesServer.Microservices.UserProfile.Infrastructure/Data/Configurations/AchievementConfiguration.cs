using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using ByteBattlesServer.Microservices.UserProfile.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("achievements");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(a => a.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(a => a.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(a => a.IconUrl)
            .HasColumnName("icon_url")
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(a => a.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
            
        builder.Property(a => a.RequiredValue)
            .HasColumnName("required_value")
            .IsRequired();
            
        builder.Property(a => a.RewardExperience)
            .HasColumnName("reward_experience")
            .IsRequired();
            
        builder.Property(a => a.IsSecret)
            .HasColumnName("is_secret")
            .IsRequired();
            
        builder.Property(a => a.Category)
            .HasColumnName("category")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);
            
        builder.Property(a => a.Rarity)
            .HasColumnName("rarity")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(a => a.UnlockMessage)
            .HasColumnName("unlock_message")
            .HasMaxLength(500);
            
    

        // Seed data - начальные достижения
       builder.HasData(
            new Achievement(
                id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                name: "Первая кровь",
                description: "Решите свою первую задачу",
                iconUrl: "/achievements/first-blood.png",
                type: AchievementType.TotalProblemsSolved,
                requiredValue: 1,
                rewardExperience: 100,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Common,
                isSecret: false,
                unlockMessage: "Отличный старт! Первая задача решена!"
            ),
            new Achievement(
                id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
                name: "Решатель",
                description: "Решите 10 задач",
                iconUrl: "/achievements/solver.png",
                type: AchievementType.TotalProblemsSolved,
                requiredValue: 10,
                rewardExperience: 250,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Common,
                isSecret: false,
                unlockMessage: null
            ),
            new Achievement(
                id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
                name: "Первая победа",
                description: "Выиграйте первый баттл",
                iconUrl: "/achievements/first-victory.png",
                type: AchievementType.Wins,
                requiredValue: 1,
                rewardExperience: 200,
                category: AchievementCategory.Battles,
                rarity: AchievementRarity.Common,
                isSecret: false,
                unlockMessage: "Поздравляем с первой победой в баттле!"
            ),
            new Achievement(
                id: Guid.Parse("44444444-4444-4444-4444-444444444444"),
                name: "Непобедимый",
                description: "Выиграйте 10 баттлов подряд",
                iconUrl: "/achievements/invincible.png",
                type: AchievementType.CurrentStreak,
                requiredValue: 10,
                rewardExperience: 1500,
                category: AchievementCategory.Streaks,
                rarity: AchievementRarity.Epic,
                isSecret: false,
                unlockMessage: null
            ),
            new Achievement(
                id: Guid.Parse("55555555-5555-5555-5555-555555555555"),
                name: "Мастер алгоритмов",
                description: "Решите 100 задач",
                iconUrl: "/achievements/algorithm-master.png",
                type: AchievementType.TotalProblemsSolved,
                requiredValue: 100,
                rewardExperience: 1000,
                category: AchievementCategory.Problems,
                rarity: AchievementRarity.Rare,
                isSecret: false,
                unlockMessage: null
            ),
            new Achievement(
                id: Guid.Parse("66666666-6666-6666-6666-666666666666"),
                name: "Ниндзя кода",
                description: "Решите задачу за 10 секунд",
                iconUrl: "/achievements/code-ninja.png",
                type: AchievementType.FastestSubmission,
                requiredValue: 10,
                rewardExperience: 2000,
                category: AchievementCategory.Special,
                rarity: AchievementRarity.Legendary,
                isSecret: true,
                unlockMessage: "Невероятная скорость! Вы настоящий ниндзя кода!"
            )
        );



        // Indexes
        builder.HasIndex(a => a.Type)
            .HasDatabaseName("ix_achievements_type");
            
        builder.HasIndex(a => a.Category)
            .HasDatabaseName("ix_achievements_category");
            
        builder.HasIndex(a => a.Rarity)
            .HasDatabaseName("ix_achievements_rarity");
            
        builder.HasIndex(a => a.RequiredValue)
            .HasDatabaseName("ix_achievements_required_value");
    }
}