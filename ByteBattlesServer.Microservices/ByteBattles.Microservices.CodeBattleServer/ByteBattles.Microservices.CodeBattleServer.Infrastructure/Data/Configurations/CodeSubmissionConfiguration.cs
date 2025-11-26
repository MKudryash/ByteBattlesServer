using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data.Configurations;

public class CodeSubmissionConfiguration : IEntityTypeConfiguration<CodeSubmission>
{
    public void Configure(EntityTypeBuilder<CodeSubmission> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.ProblemId)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(s => s.Code)
            .IsRequired();
            
        builder.OwnsOne(s => s.Result, result =>
        {
            result.Property(r => r.Message).HasMaxLength(500);
            result.Property(r => r.IsSuccess);
            result.Property(r => r.TestsPassed);
            result.Property(r => r.TotalTests);
            result.Property(r => r.ExecutionTime);
        });
        
        builder.ToTable("code_submissions");
    }
}