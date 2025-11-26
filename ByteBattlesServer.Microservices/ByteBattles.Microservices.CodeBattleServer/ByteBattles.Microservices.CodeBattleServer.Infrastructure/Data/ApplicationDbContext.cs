using ByteBattles.Microservices.CodeBattleServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ByteBattles.Microservices.CodeBattleServer.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<BattleRoom> BattleRooms { get; set; }
    public DbSet<CodeSubmission> CodeSubmissions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
}