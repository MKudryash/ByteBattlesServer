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
            
        builder.Property(ua => ua.AchievedAt)
            .HasColumnName("achieved_at")
            .IsRequired();

        // Composite unique index
        builder.HasIndex(ua => new { ua.UserProfileId, ua.AchievementId })
            .IsUnique();

        // Relationships
        builder.HasOne(ua => ua.UserProfile)
            .WithMany(up => up.Achievements)
            .HasForeignKey(ua => ua.UserProfileId);

        builder.HasOne(ua => ua.Achievement)
            .WithMany()
            .HasForeignKey(ua => ua.AchievementId);
    }
}