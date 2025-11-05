namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Configurations;
using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(l => l.ShortTitle)
            .IsRequired()
            .HasMaxLength(10); 
        
        builder.Property(l => l.FileExtension)
            .IsRequired()
            .HasMaxLength(10); 
        
        builder.Property(l => l.CompilerCommand)
            .IsRequired()
            .HasMaxLength(50); // Ограничение длины
    
        builder.Property(l => l.ExecutionCommand)
            .IsRequired()
            .HasMaxLength(50);
    
        builder.Property(l => l.SupportsCompilation)
            .IsRequired()
            .HasDefaultValue(false); // Значение по умолчанию
        
        builder.HasMany(l => l.TasksLanguage)
            .WithOne(tl => tl.Language)
            .HasForeignKey(tl => tl.IdLanguage)
            .OnDelete(DeleteBehavior.Restrict); 
        
        builder.HasIndex(l => l.Title)
            .IsUnique();
            
        builder.HasIndex(l => l.ShortTitle)
            .IsUnique();
    }
}