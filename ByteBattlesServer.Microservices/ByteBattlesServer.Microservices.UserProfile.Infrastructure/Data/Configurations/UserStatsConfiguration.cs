using System.Text.Json;
using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class UserStatsConfiguration : IEntityTypeConfiguration<UserStats>
{
    public void Configure(EntityTypeBuilder<UserStats> builder)
    {
        builder.ToTable("user_stats");
        
        builder.HasKey(us => us.Id);
        
        builder.Property(up => up.Id)
            .HasColumnName("id")
            .IsRequired();

            
        builder.Property(us => us.TotalProblemsSolved)
            .HasColumnName("total_problems_solved")
            .HasDefaultValue(0);
            
        builder.Property(us => us.TotalBattles)
            .HasColumnName("total_battles")
            .HasDefaultValue(0);
            
        builder.Property(us => us.Wins)
            .HasColumnName("wins")
            .HasDefaultValue(0);
            
        builder.Property(us => us.Losses)
            .HasColumnName("losses")
            .HasDefaultValue(0);
            
        builder.Property(us => us.Draws)
            .HasColumnName("draws")
            .HasDefaultValue(0);
            
        builder.Property(us => us.CurrentStreak)
            .HasColumnName("current_streak")
            .HasDefaultValue(0);
            
        builder.Property(us => us.MaxStreak)
            .HasColumnName("max_streak")
            .HasDefaultValue(0);
            
        builder.Property(us => us.TotalExperience)
            .HasColumnName("total_experience")
            .HasDefaultValue(0);
            
        builder.Property(us => us.EasyProblemsSolved)
            .HasColumnName("easy_problems_solved")
            .HasDefaultValue(0);
            
        builder.Property(us => us.MediumProblemsSolved)
            .HasColumnName("medium_problems_solved")
            .HasDefaultValue(0);
            
        builder.Property(us => us.HardProblemsSolved)
            .HasColumnName("hard_problems_solved")
            .HasDefaultValue(0);
            
        builder.Property(us => us.TotalSubmissions)
            .HasColumnName("total_submissions")
            .HasDefaultValue(0);
            
        builder.Property(us => us.SuccessfulSubmissions)
            .HasColumnName("successful_submissions")
            .HasDefaultValue(0);
            
        builder.Property(us => us.TotalExecutionTime)
            .HasColumnName("total_execution_time")
            .HasConversion(
                v => v.Ticks,
                v => TimeSpan.FromTicks(v))
            .HasDefaultValue(TimeSpan.Zero);

        builder.Property(us => us.SolvedTaskIds)
            .HasColumnName("solved_task_ids")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize<HashSet<Guid>>(v, (JsonSerializerOptions)null),
                v => string.IsNullOrEmpty(v) 
                    ? new HashSet<Guid>() 
                    : System.Text.Json.JsonSerializer.Deserialize<HashSet<Guid>>(v, (JsonSerializerOptions)null))
            .IsRequired(false)
            .HasColumnType("jsonb");

        builder.Property(us => us.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
    }
}