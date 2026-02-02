using Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace SnookerLive;

public class WatchOnDbContext : DbContext
{
    public WatchOnDbContext(DbContextOptions<WatchOnDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<WatchOn> WatchOn { get; set; } = null!;

    public async Task EnsureIndexesCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WatchOn>(entity =>
        {
            entity.ToCollection("watch");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}