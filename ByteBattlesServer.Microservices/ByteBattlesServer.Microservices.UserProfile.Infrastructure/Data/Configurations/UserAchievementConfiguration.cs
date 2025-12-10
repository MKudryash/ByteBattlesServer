using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("user_achievements");
        
        builder.HasKey(ua => ua.Id);
        
        builder.Property(ua => ua.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(ua => ua.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
            
        builder.Property(ua => ua.AchievementId)
            .HasColumnName("achievement_id")
            .IsRequired();
            
        builder.Property(ua => ua.Progress)
            .HasColumnName("progress")
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(ua => ua.IsUnlocked)
            .HasColumnName("is_unlocked")
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(ua => ua.UnlockedAt)
            .HasColumnName("unlocked_at")
            .IsRequired(false);
            
        
        // Relationships
        builder.HasOne(ua => ua.UserProfile)
            .WithMany(up => up.Achievements)
            .HasForeignKey(ua => ua.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_user_achievements_user_profiles");
            
        builder.HasOne(ua => ua.Achievement)
            .WithMany()
            .HasForeignKey(ua => ua.AchievementId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_user_achievements_achievements");

        // Indexes
        builder.HasIndex(ua => new { ua.UserProfileId, ua.AchievementId })
            .IsUnique()
            .HasDatabaseName("ix_user_achievements_user_profile_achievement");
            
        builder.HasIndex(ua => ua.UserProfileId)
            .HasDatabaseName("ix_user_achievements_user_profile");
            
        builder.HasIndex(ua => ua.AchievementId)
            .HasDatabaseName("ix_user_achievements_achievement");
            
        builder.HasIndex(ua => ua.IsUnlocked)
            .HasDatabaseName("ix_user_achievements_is_unlocked");
            
        builder.HasIndex(ua => ua.UnlockedAt)
            .HasDatabaseName("ix_user_achievements_unlocked_at")
            .HasFilter("unlocked_at IS NOT NULL");
    }
}