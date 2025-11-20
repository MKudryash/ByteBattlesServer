using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
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

        builder.Property(up => up.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired(false);
        
            
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

        builder.Property(up => up.Role)
            .HasColumnName("Role")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);
            
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

        builder.HasOne(up => up.Stats)
            .WithOne(us => us.UserProfile) // Add navigation property
            .HasForeignKey<UserStats>(us => us.UserProfileId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.TeacherStats)
            .WithOne(ts => ts.UserProfile) // Add navigation property
            .HasForeignKey<TeacherStats>(ts => ts.UserProfileId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Settings)
            .WithOne(us => us.UserProfile) // Add navigation property
            .HasForeignKey<UserSettings>(us => us.UserProfileId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);


        // Relationships
        builder.HasMany(up => up.Achievements)
            .WithOne(ua => ua.UserProfile)
            .HasForeignKey(ua => ua.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(up => up.BattleHistory)
            .WithOne(br => br.UserProfile)
            .HasForeignKey(br => br.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.RecentActivities)
            .WithOne(ra => ra.UserProfile)
            .HasForeignKey(ra => ra.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RecentProblems)
            .WithOne(rp => rp.UserProfile)
            .HasForeignKey(rp => rp.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(up => up.UserId)
            .IsUnique();
            
        builder.HasIndex(up => up.UserName);
        builder.HasIndex(up => up.Email);
        builder.HasIndex(up => up.Role);
        builder.HasIndex(up => up.IsPublic);
        builder.HasIndex(up => up.Level);
        builder.HasIndex(up => up.CreatedAt);
        builder.HasIndex(up => up.UpdatedAt);

        // Частичные индексы для оптимизации запросов по ролям
        builder.HasIndex(up => up.Role)
            .HasFilter("\"Role\" = 'Student'")
            .HasDatabaseName("ix_user_profiles_role_student");

        builder.HasIndex(up => up.Role)
            .HasFilter("\"Role\" = 'Teacher'")
            .HasDatabaseName("ix_user_profiles_role_teacher");
    }
}