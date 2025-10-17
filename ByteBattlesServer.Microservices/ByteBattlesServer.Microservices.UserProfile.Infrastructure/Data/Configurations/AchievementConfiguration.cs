using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
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

        // Indexes
        builder.HasIndex(a => a.Type);
        builder.HasIndex(a => a.RequiredValue);
    }
}