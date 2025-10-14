// ByteBattlesServer.Microservices.AuthService.Infrastructure/Data/Configurations/RefreshTokenConfiguration.cs
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
            .HasMaxLength(45)
            .IsRequired(false); // Разрешаем NULL
        
        builder.Property(rt => rt.Revoked)
            .IsRequired(false); // Разрешаем NULL
            
        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(45)
            .IsRequired(false); // Разрешаем NULL - ЭТО ГЛАВНОЕ ИЗМЕНЕНИЕ
            
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500)
            .IsRequired(false); // Разрешаем NULL

        // Индексы
        builder.HasIndex(rt => rt.Token)
            .IsUnique();
            
        builder.HasIndex(rt => rt.UserId);

        // Внешний ключ
        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId);
    }
}