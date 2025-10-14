using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Data.Configurations;


public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasColumnName("id")
            .IsRequired();
            
        builder.Property(r => r.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}