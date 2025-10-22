using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Configurations;

public class TestsTaskConfiguration : IEntityTypeConfiguration<TestCases>
{
    public void Configure(EntityTypeBuilder<TestCases> builder)
    {
        builder.ToTable("TestsTasks");
        
        builder.HasKey(tt => tt.Id);
        
        builder.Property(tt => tt.Input)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(tt => tt.ExpectedOutput)
            .IsRequired()
            .HasMaxLength(1000);
            
        builder.Property(tt => tt.CreatedAd)
            .IsRequired();
            
        builder.Property(tt => tt.UpdatedAd)
            .IsRequired();

       
        builder.HasIndex(tt => tt.TaskId);
    }
}