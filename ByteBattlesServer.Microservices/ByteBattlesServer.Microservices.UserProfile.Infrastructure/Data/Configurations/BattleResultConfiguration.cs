using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;


public class BattleResultConfiguration : IEntityTypeConfiguration<BattleResult>
{
    public void Configure(EntityTypeBuilder<BattleResult> builder)
    {
        builder.ToTable("battle_results");
        
        builder.HasKey(br => br.Id);
        
        builder.Property(br => br.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(br => br.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
            
        builder.Property(br => br.BattleId)
            .HasColumnName("battle_id")
            .IsRequired();
            
        builder.Property(br => br.OpponentName)
            .HasColumnName("opponent_name")
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(br => br.Result)
            .HasColumnName("result")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(br => br.ExperienceGained)
            .HasColumnName("experience_gained")
            .IsRequired();
            
        builder.Property(br => br.ProblemSolved)
            .HasColumnName("problems_solved")
            .IsRequired();
            
        builder.Property(br => br.CompletionTime)
            .HasColumnName("completion_time")
            .IsRequired()
            .HasConversion(
                v => v.Ticks,
                v => TimeSpan.FromTicks(v));
            
        builder.Property(br => br.BattleDate)
            .HasColumnName("battle_date")
            .IsRequired();

        // Relationships
        builder.HasOne(br => br.UserProfile)
            .WithMany(up => up.BattleHistory)
            .HasForeignKey(br => br.UserProfileId);

        // Indexes
        builder.HasIndex(br => br.UserProfileId);
        builder.HasIndex(br => br.BattleId);
        builder.HasIndex(br => br.BattleDate);
    }
}