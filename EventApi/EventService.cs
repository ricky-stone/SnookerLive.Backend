using Domain;
using Microsoft.EntityFrameworkCore;

namespace EventApi;

public interface IEventService
{
    Task<EventRecord?> GetEventByIdAsync(string id);
    Task<List<EventRecord>> GetAllEventsForSeasonAsync(int season);
    Task<List<EventRecord>> GetUpcomingEventsForSeasonAsync(int season, int? nextDays);
    Task<List<EventRecord>> GetLiveEventsForSeasonAsync(int season);
    Task<List<EventRecord>> GetFinishedEventsForSeasonAsync(int season, int? lastDays);
    Task<bool> AddAsync(EventRecord @event);
    Task<bool> UpdateAsync(EventRecord @event);
}

public class EventService(EventDbContext db) : IEventService
{
    public async Task<EventRecord?> GetEventByIdAsync(string id) =>
        await db.Events.FindAsync(id);

    public async Task<List<EventRecord>> GetAllEventsForSeasonAsync(int season) =>
        await db.Events.Where(e => e.Season == season).ToListAsync();

    public async Task<List<EventRecord>> GetUpcomingEventsForSeasonAsync(int season, int? nextDays)
    {
        var now = DateTime.UtcNow;

        var query = db.Events
            .Where(e =>
                e.Season == season &&
                e.StartDate > now &&
                e.NumUpcoming > 0);

        if (nextDays is not null)
        {
            var to = now.AddDays(nextDays.Value);
            query = query.Where(e => e.StartDate <= to);
        }

        return await query
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<List<EventRecord>> GetLiveEventsForSeasonAsync(int season)
    {
        var now = DateTimeOffset.UtcNow;

        return await db.Events
            .Where(e => e.Season == season
                        && e.StartDate <= now
                        && e.EndDate > now)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<List<EventRecord>> GetFinishedEventsForSeasonAsync(int season, int? lastDays)
    {
        var now = DateTimeOffset.UtcNow;

        var query = db.Events
            .Where(e => e.Season == season
                        && e.EndDate <= now);

        if (lastDays is not null)
        {
            var from = now.AddDays(-lastDays.Value);
            query = query.Where(e => e.EndDate >= from);
        }

        return await query
            .OrderByDescending(e => e.EndDate)
            .ToListAsync();
    }

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