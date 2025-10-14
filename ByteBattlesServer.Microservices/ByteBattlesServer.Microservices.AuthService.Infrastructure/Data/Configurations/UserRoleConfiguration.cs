using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Data.Configurations;


public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");
        
        builder.HasKey(ur => ur.Id);
        
        builder.Property(ur => ur.UserId)
            .IsRequired();
            
        builder.Property(ur => ur.RoleId)
            .IsRequired();

        // Составной уникальный индекс
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // Внешние ключи
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
    }
}