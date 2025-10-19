using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Data.Configurations;


public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);
        
        builder.OwnsOne(u => u.Email, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("Email"); 
            
            ownedNavigationBuilder.HasIndex(e => e.Value)
                .IsUnique();
        });
            
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
            
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.IsActive)
            .IsRequired();
            
        builder.Property(u => u.CreatedAt)
            .IsRequired();
            
        builder.Property(u => u.UpdatedAt);


        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            ;
    }
}