using Domain;
using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public interface IWatchOnService
{
    Task<WatchOn?> GetWatchOnByIdAsync(string id);
    Task<WatchOn?> GetAllForEventAsync(string eventId);
    Task UpsertAsync(WatchOn watchOn);
}

public sealed class WatchOnService(WatchOnDbContext db) : IWatchOnService
{
    public Task<WatchOn?> GetWatchOnByIdAsync(string id) =>
        db.WatchOn.FindAsync(id).AsTask();
    
    public async Task<WatchOn?> GetAllForEventAsync(string eventId) =>
        await db.WatchOn.Where(w => w.Id == eventId).FirstOrDefaultAsync();

    public async Task UpsertAsync(WatchOn watchOn)
    {
        var existing = await db.WatchOn.FindAsync(watchOn.Id);
        if (existing is null)
            db.WatchOn.Add(watchOn);
        else
            db.Entry(existing).CurrentValues.SetValues(watchOn);

        await db.SaveChangesAsync();
    }
}