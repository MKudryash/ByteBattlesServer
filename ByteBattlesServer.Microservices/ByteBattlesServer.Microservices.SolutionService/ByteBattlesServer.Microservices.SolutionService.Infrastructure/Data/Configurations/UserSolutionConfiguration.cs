using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data.Configurations;

public class UserSolutionConfiguration :IEntityTypeConfiguration<UserSolution>
{
        public void Configure(EntityTypeBuilder<UserSolution> builder)
        {


                builder.HasKey(s => s.Id);

                builder.Property(s => s.Code)
                        .IsRequired()
                        .HasMaxLength(10000);

                builder.Property(s => s.Language)
                        .IsRequired()
                        .HasMaxLength(50);

                builder.Property(s => s.Status)
                        .IsRequired()
                        .HasConversion<string>();

                builder.Property(s => s.ErrorMessage)
                        .HasMaxLength(2000);

                builder.Property(s => s.ExecutionResult)
                        .HasMaxLength(4000);
                builder.HasIndex(s => s.UserId);
                builder.HasIndex(s => s.TaskId);
                builder.HasIndex(s => new { s.UserId, s.TaskId });
                builder.HasIndex(s => s.Status);
                builder.HasIndex(s => s.SubmittedAt);
                builder.HasIndex(s => new { s.UserId, s.TaskId, s.AttemptNumber }).IsUnique();
                builder.HasOne<UserTaskStats>()
                        .WithMany()
                        .HasForeignKey(s => new { s.UserId, s.TaskId })
                        .HasPrincipalKey(uts => new { uts.UserId, uts.TaskId });
               
        }
}

public class UserTaskStatsConfiguration : IEntityTypeConfiguration<UserTaskStats>
{
        public void Configure(EntityTypeBuilder<UserTaskStats> builder)
        {
                builder.HasKey(s => s.Id);
                
                builder.HasIndex(s => new { s.UserId, s.TaskId }).IsUnique();
            
                builder.Property(s => s.BestExecutionTime)
                        .HasConversion(
                                v => v.Ticks,
                                v => TimeSpan.FromTicks(v));
        }
}