using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Configurations;

public class LibraryConfiguration : IEntityTypeConfiguration<Library>
{
    public void Configure(EntityTypeBuilder<Library> builder)
    {
        builder.ToTable("Libraries");
        
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.NameLibrary)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(100);
        
        
        builder.Property(l => l.Version)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(l => l.CreatedAd)
            .IsRequired();
            
        builder.Property(l => l.UpdatedAd)
            .IsRequired();

       
        builder.HasIndex(l=> l.LanguageId);
        builder.HasIndex(t => t.CreatedAd);
        
        builder.HasMany(t => t.Libraries)
            .WithOne(tt => tt.Library)
            .HasForeignKey(tt => tt.IdLibrary)
            .OnDelete(DeleteBehavior.Cascade);
        
    }
}