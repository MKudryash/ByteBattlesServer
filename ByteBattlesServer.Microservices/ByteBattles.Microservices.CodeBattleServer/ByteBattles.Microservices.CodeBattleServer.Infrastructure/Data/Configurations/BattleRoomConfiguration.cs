using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data.Configurations;

public class BattleRoomConfiguration : IEntityTypeConfiguration<BattleRoom>
{
    public void Configure(EntityTypeBuilder<BattleRoom> builder)
    {
        builder.ToTable("battle_rooms");

        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Конфигурация для Participants как owned entity collection
        builder.OwnsMany(r => r.Participants, participant =>
        {
            participant.WithOwner().HasForeignKey("RoomId");
            
            // Явно определяем первичный ключ для owned entity
            participant.Property<Guid>("Id");
            participant.HasKey("Id");
            
            participant.Property(p => p.UserId)
                .IsRequired();
                
            participant.Property(p => p.JoinedAt)
                .IsRequired();
                
            participant.ToTable("room_participants");
        });

        // Настройка навигационного свойства Submissions
        builder.HasMany(r => r.Submissions)
            .WithOne()
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Указываем использовать поле для навигационных свойств
        builder.Navigation(r => r.Participants)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
            
        builder.Navigation(r => r.Submissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}