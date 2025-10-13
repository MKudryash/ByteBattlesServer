using ByteBattlesServer.Microservices.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.AuthService.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        
        builder.HasKey(rt => rt.Id);
        
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(rt => rt.Expires)
            .IsRequired();
            
        builder.Property(rt => rt.Created)
            .IsRequired();
            
        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(45);
            
        builder.Property(rt => rt.Revoked);
            
        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(45);
            
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId);
    }
}