using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class RecentProblemConfiguration : IEntityTypeConfiguration<RecentProblem>
{
    public void Configure(EntityTypeBuilder<RecentProblem> builder)
    {
        builder.ToTable("recent_problems");
        
        builder.HasKey(rp => rp.Id);
        
        builder.Property(rp => rp.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(rp => rp.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
            
        builder.Property(rp => rp.ProblemId)
            .HasColumnName("problem_id")
            .IsRequired();
            
        builder.Property(rp => rp.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(rp => rp.Difficulty)
            .HasColumnName("difficulty")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(rp => rp.Language)
            .HasColumnName("language")
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(rp => rp.SolvedAt)
            .HasColumnName("solved_at")
            .IsRequired();
            
        builder.HasIndex(rp => rp.UserProfileId);
        builder.HasIndex(rp => rp.ProblemId);
        builder.HasIndex(rp => rp.Difficulty);
        builder.HasIndex(rp => rp.SolvedAt);
    }
}