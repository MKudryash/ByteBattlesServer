namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Configurations;

using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("Tasks"); 
        
        builder.HasKey(t => t.Id);
        
        // Свойства
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);
            
        builder.Property(t => t.Difficulty)
            .IsRequired()
            .HasConversion<string>() 
            .HasMaxLength(20);
            
        builder.Property(t => t.Author)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(t => t.FunctionName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(t => t.PatternFunction)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(t => t.PatternMain)
            .IsRequired()
            .HasMaxLength(500);

            
        builder.Property(t => t.CreatedAt)
            .IsRequired();
            
        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

     
        builder.HasMany(t => t.TaskLanguages)
            .WithOne(tl => tl.Task)
            .HasForeignKey(tl => tl.IdTask)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(t => t.TestCases)
            .WithOne(tt => tt.Task)
            .HasForeignKey(tt => tt.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
      
        builder.HasIndex(t => t.Difficulty);
        builder.HasIndex(t => t.Author);
        builder.HasIndex(t => t.CreatedAt);
    }
}