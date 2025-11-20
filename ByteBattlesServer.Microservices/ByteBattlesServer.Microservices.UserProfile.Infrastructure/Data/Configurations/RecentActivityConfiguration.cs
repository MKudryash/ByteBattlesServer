using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class RecentActivityConfiguration : IEntityTypeConfiguration<RecentActivity>
{
    public void Configure(EntityTypeBuilder<RecentActivity> builder)
    {
        builder.ToTable("recent_activities");
        
        builder.HasKey(ra => ra.Id);
        
        builder.Property(ra => ra.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(ra => ra.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();
            
        builder.Property(ra => ra.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
            
        builder.Property(ra => ra.Title)
            .HasColumnName("title")
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(ra => ra.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(ra => ra.Timestamp)
            .HasColumnName("timestamp")
            .IsRequired();
            
        builder.Property(ra => ra.ExperienceGained)
            .HasColumnName("experience_gained")
            .HasDefaultValue(0);

        // Relationships
        builder.HasOne(ra => ra.UserProfile)
            .WithMany(up => up.RecentActivities)
            .HasForeignKey(ra => ra.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(ra => ra.UserProfileId);
        builder.HasIndex(ra => ra.Type);
        builder.HasIndex(ra => ra.Timestamp);
        builder.HasIndex(ra => new { ra.UserProfileId, ra.Timestamp });
    }
}