// UserProfile.Infrastructure/Data/UserProfileDbContext.cs

using ByteBattlesServer.Microservices.UserProfile.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ByteBattlesServer.Microservices.UserProfile.Infrastructure.Data;

public class UserProfileDbContext : DbContext
{
    public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.UserProfile> UserProfiles { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<BattleResult> BattleResults { get; set; }
    public DbSet<RecentActivity> RecentActivities { get; set; }
    public DbSet<RecentProblem> RecentProblems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserProfileDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Domain.Entities.UserProfile && 
                        e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Modified)
            {
                // Используем рефлексию для установки UpdatedAt
                var userProfile = (Domain.Entities.UserProfile)entityEntry.Entity;
                var updatedAtProperty = typeof(Domain.Entities.UserProfile).GetProperty("UpdatedAt");
                updatedAtProperty?.SetValue(userProfile, DateTime.UtcNow);
            }
        }
    }
}