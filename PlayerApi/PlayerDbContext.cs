using Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace SnookerLive;

public class PlayerDbContext : DbContext
{
    public PlayerDbContext(DbContextOptions<PlayerDbContext> options) : base(options) 
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<PlayerRecord> Players { get; set; } = null!;

    public async Task EnsureIndexesCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlayerRecord>(entity =>
        {
            entity.ToCollection("all");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}