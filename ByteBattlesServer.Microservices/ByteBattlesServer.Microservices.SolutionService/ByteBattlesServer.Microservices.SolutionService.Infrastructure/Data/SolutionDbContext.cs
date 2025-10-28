using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;


public class SolutionDbContext : DbContext
{
    public SolutionDbContext(DbContextOptions<SolutionDbContext> options) : base(options)
    {
    }

    public DbSet<Solution> Solutions { get; set; }
    public DbSet<SolutionAttempt> SolutionAttempts { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Solution>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Status).HasConversion<string>();
            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => s.TaskId);
            entity.HasIndex(s => new { s.TaskId, s.UserId });
            
            entity.HasMany(s => s.TestResults)
                .WithOne()
                .HasForeignKey(tr => tr.SolutionId);
                  
            entity.HasMany(s => s.Attempts)
                .WithOne()
                .HasForeignKey(a => a.SolutionId);
        });

        modelBuilder.Entity<SolutionAttempt>(entity =>
        {
            entity.HasKey(sa => sa.Id);
            entity.Property(sa => sa.Status).HasConversion<string>();
            entity.HasIndex(sa => sa.SolutionId);
        });

        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(tr => tr.Id);
            entity.Property(tr => tr.Status).HasConversion<string>();
            entity.HasIndex(tr => tr.SolutionId);
            entity.HasIndex(tr => tr.TestCaseId);
        });
    }
}