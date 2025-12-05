using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data.Configurations;

public class TeacherStatsConfiguration : IEntityTypeConfiguration<TeacherStats>
{
    public void Configure(EntityTypeBuilder<TeacherStats> builder)
    {
        builder.ToTable("teacher_stats");
        
        builder.HasKey(ts => ts.Id);
        
        builder.Property(ts => ts.Id)
            .HasColumnName("id")
            .IsRequired();

   
            
        builder.Property(ts => ts.CreatedTasks)
            .HasColumnName("created_tasks")
            .HasDefaultValue(0);
                
        builder.Property(ts => ts.ActiveStudents)
            .HasColumnName("active_students")
            .HasDefaultValue(0);
                
        builder.Property(ts => ts.AverageRating)
            .HasColumnName("average_rating")
            .HasPrecision(3, 2)
            .HasDefaultValue(0.0);
                
        builder.Property(ts => ts.TotalSubmissions)
            .HasColumnName("total_submissions")
            .HasDefaultValue(0);
        builder.Property(ts => ts.UserProfileId)
            .HasColumnName("user_profile_id")
            .IsRequired();


    }
}