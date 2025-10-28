using ByteBattlesServer.Microservices.SolutionService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.SolutionService.Infrastructure.Data;

public class SolutionDbContext: DbContext
{
    public SolutionDbContext(DbContextOptions<SolutionDbContext> options) : base(options)
    {
    }

    public DbSet<UserSolution> UserSolutions { get; set; }
    public DbSet<UserTaskStats> UserTaskStats { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SolutionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

}