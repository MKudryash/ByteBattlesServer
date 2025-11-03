using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<Domain.Entities.UserProfile>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        
        builder.HasKey(up => up.Id);
        
        builder.Property(up => up.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(up => up.UserId)
            .HasColumnName("user_id")
            .IsRequired();
            
        builder.Property(up => up.UserName)
            .HasColumnName("user_name")
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(up => up.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(500)
            .IsRequired(false);
            
        builder.Property(up => up.Bio)
            .HasColumnName("bio")
            .HasMaxLength(500)
            .IsRequired(false);
            
        builder.Property(up => up.Country)
            .HasColumnName("country")
            .HasMaxLength(100)
            .IsRequired(false);
            
        builder.Property(up => up.GitHubUrl)
            .HasColumnName("github_url")
            .HasMaxLength(200)
            .IsRequired(false);
            
        builder.Property(up => up.LinkedInUrl)
            .HasColumnName("linkedin_url")
            .HasMaxLength(200)
            .IsRequired(false);
            
        builder.Property(up => up.Level)
            .HasColumnName("level")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(up => up.IsPublic)
            .HasColumnName("is_public")
            .IsRequired();
            
        builder.Property(up => up.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
            
        builder.Property(up => up.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Owned type for UserStats with proper column names
        builder.OwnsOne(up => up.Stats, stats =>
        {
            stats.ToTable("user_stats"); // Optional: separate table
            
            stats.Property(s => s.TotalProblemsSolved)
                .HasColumnName("total_problems_solved")
                .HasDefaultValue(0);
                
            stats.Property(s => s.TotalBattles)
                .HasColumnName("total_battles")
                .HasDefaultValue(0);
                
            stats.Property(s => s.Wins)
                .HasColumnName("wins")
                .HasDefaultValue(0);
                
            stats.Property(s => s.Losses)
                .HasColumnName("losses")
                .HasDefaultValue(0);
                
            stats.Property(s => s.Draws)
                .HasColumnName("draws")
                .HasDefaultValue(0);
                
            stats.Property(s => s.CurrentStreak)
                .HasColumnName("current_streak")
                .HasDefaultValue(0);
                
            stats.Property(s => s.MaxStreak)
                .HasColumnName("max_streak")
                .HasDefaultValue(0);
                
            stats.Property(s => s.TotalExperience)
                .HasColumnName("total_experience")
                .HasDefaultValue(0);
                
            stats.Property(s => s.EasyProblemsSolved)
                .HasColumnName("easy_problems_solved")
                .HasDefaultValue(0);
                
            stats.Property(s => s.MediumProblemsSolved)
                .HasColumnName("medium_problems_solved")
                .HasDefaultValue(0);
                
            stats.Property(s => s.HardProblemsSolved)
                .HasColumnName("hard_problems_solved")
                .HasDefaultValue(0);
                
            stats.Property(s => s.TotalSubmissions)
                .HasColumnName("total_submissions")
                .HasDefaultValue(0);
                
            stats.Property(s => s.SuccessfulSubmissions)
                .HasColumnName("successful_submissions")
                .HasDefaultValue(0);
                
            stats.Property(s => s.TotalExecutionTime)
                .HasColumnName("total_execution_time")
                .HasConversion(
                    v => v.Ticks,
                    v => TimeSpan.FromTicks(v))
                .HasDefaultValue(TimeSpan.Zero);

            // Store SolvedTaskIds as JSON or in separate table
            stats.Property(s => s.SolvedTaskIds)
                .HasColumnName("solved_task_ids")
                .HasConversion(
                    v => string.Join(',', v),
                    v => new HashSet<Guid>(v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(Guid.Parse)))
                .IsRequired(false);
        });

        // Owned type for UserSettings
        builder.OwnsOne(up => up.Settings, settings =>
        {
            settings.Property(s => s.EmailNotifications)
                .HasColumnName("email_notifications")
                .HasDefaultValue(true);
                
            settings.Property(s => s.BattleInvitations)
                .HasColumnName("battle_invitations")
                .HasDefaultValue(true);
                
            settings.Property(s => s.AchievementNotifications)
                .HasColumnName("achievement_notifications")
                .HasDefaultValue(true);
                
            settings.Property(s => s.Theme)
                .HasColumnName("theme")
                .HasMaxLength(50)
                .HasDefaultValue("light");
                
            settings.Property(s => s.CodeEditorTheme)
                .HasColumnName("code_editor_theme")
                .HasMaxLength(50)
                .HasDefaultValue("vs-light");
                
            settings.Property(s => s.PreferredLanguage)
                .HasColumnName("preferred_language")
                .HasMaxLength(50)
                .HasDefaultValue("csharp");
        });

        // Relationships
        builder.HasMany(up => up.Achievements)
            .WithOne(ua => ua.UserProfile)
            .HasForeignKey(ua => ua.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(up => up.BattleHistory)
            .WithOne(br => br.UserProfile)
            .HasForeignKey(br => br.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(up => up.UserId)
            .IsUnique();
            
        builder.HasIndex(up => up.UserName);
        builder.HasIndex(up => up.IsPublic);
        builder.HasIndex(up => up.Level);
        builder.HasIndex(up => up.CreatedAt);
        builder.HasIndex(up => up.UpdatedAt);
    }
}