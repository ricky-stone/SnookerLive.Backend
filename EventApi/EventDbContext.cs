using Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace EventApi;

public class EventDbContext : DbContext
{
    public EventDbContext(DbContextOptions<EventDbContext> options) : base(options) 
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<EventRecord> Events { get; set; } = null!;

    public async Task EnsureIndexesCreatedAsync()
    {
        await Database.EnsureCreatedAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventRecord>(entity =>
        {
            entity.ToCollection("all");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Season);
            entity.Property(e => e.Id).ValueGeneratedNever();
        });
    }
}