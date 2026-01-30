using Domain;
using Microsoft.EntityFrameworkCore;

namespace EventApi;

public interface IEventService
{
    Task<EventRecord?> GetEventByIdAsync(string id);
    Task<List<EventRecord>> GetAllEventsForSeasonAsync(int season);
    Task<bool> AddAsync(EventRecord @event);
    Task<bool> UpdateAsync(EventRecord @event);
}

public class EventService(EventDbContext db) : IEventService
{
    public async Task<EventRecord?> GetEventByIdAsync(string id) =>
        await db.Events.FindAsync(id);
    
    public async Task<List<EventRecord>> GetAllEventsForSeasonAsync(int season) =>
        await db.Events.Where(e => e.Season == season).ToListAsync();

    public async Task<bool> AddAsync(EventRecord @event)
    {
        await db.Events.AddAsync(@event);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(EventRecord @event)
    {
        var existing = await db.Events
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == @event.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(@event);
        await db.SaveChangesAsync();
        return true;
    }
}