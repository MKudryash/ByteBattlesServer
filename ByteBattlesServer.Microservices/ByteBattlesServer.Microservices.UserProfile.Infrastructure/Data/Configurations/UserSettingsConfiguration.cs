using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.ToTable("user_settings");
        
        builder.HasKey(us => us.Id);
        
        builder.Property(us => us.Id)
            .HasColumnName("id")
            .IsRequired();

      
            
        builder.Property(us => us.EmailNotifications)
            .HasColumnName("email_notifications")
            .HasDefaultValue(true);
                
        builder.Property(us => us.BattleInvitations)
            .HasColumnName("battle_invitations")
            .HasDefaultValue(true);
                
        builder.Property(us => us.AchievementNotifications)
            .HasColumnName("achievement_notifications")
            .HasDefaultValue(true);
                
        builder.Property(us => us.Theme)
            .HasColumnName("theme")
            .HasMaxLength(50)
            .HasDefaultValue("light");
                
        builder.Property(us => us.CodeEditorTheme)
            .HasColumnName("code_editor_theme")
            .HasMaxLength(50)
            .HasDefaultValue("vs-light");
                
        builder.Property(us => us.PreferredLanguage)
            .HasColumnName("preferred_language")
            .HasMaxLength(50)
            .HasDefaultValue("csharp");
        builder.Property(us => us.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
    }
}