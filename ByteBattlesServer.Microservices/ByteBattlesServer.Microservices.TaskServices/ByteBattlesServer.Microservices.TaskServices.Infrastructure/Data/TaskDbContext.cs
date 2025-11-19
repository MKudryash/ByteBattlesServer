using ByteBattlesServer.Microservices.TaskServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = ByteBattlesServer.Microservices.TaskServices.Domain.Entities.Task;


namespace ByteBattlesServer.Microservices.TaskServices.Infrastructure.Data;

public class TaskDbContext: DbContext
{
    
    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }
    
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Language> Language { get; set; }
    public DbSet<TaskLanguage> TaskLanguages { get; set; }
    public DbSet<TestCases> TestsTasks { get; set; }
    public DbSet<Library> Libraries { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDbContext).Assembly);
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
            .Where(e => e.Entity is Domain.Entities.Task && 
                        e.State == EntityState.Modified);

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Modified)
            {
                // Используем рефлексию для установки UpdatedAt
                var userProfile = (Domain.Entities.Task)entityEntry.Entity;
                var updatedAtProperty = typeof(Domain.Entities.Task).GetProperty("UpdatedAt");
                updatedAtProperty?.SetValue(userProfile, DateTime.UtcNow);
            }
        }
    }
}