namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Configurations;

using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class TaskLanguageConfiguration : IEntityTypeConfiguration<TaskLanguage>
{
    public void Configure(EntityTypeBuilder<TaskLanguage> builder)
    {
        builder.ToTable("TaskLanguages");
        
       
        builder.HasKey(tl => tl.Id);
        
        
        builder.HasIndex(tl => new { tl.IdTask, tl.IdLanguage })
            .IsUnique();
        
        
        builder.HasOne(tl => tl.Task)
            .WithMany(t => t.TaskLanguages)
            .HasForeignKey(tl => tl.IdTask)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(tl => tl.Language)
            .WithMany(l => l.TasksLanguage)
            .HasForeignKey(tl => tl.IdLanguage)
            .OnDelete(DeleteBehavior.Restrict); 
            
        
        builder.Property(tl => tl.IdTask)
            .IsRequired();
            
        builder.Property(tl => tl.IdLanguage)
            .IsRequired();
    }
}