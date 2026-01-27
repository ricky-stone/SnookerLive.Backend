using Domain;
using Microsoft.EntityFrameworkCore;

namespace SessionApi;

public interface ISessionService
{
    Task<SessionRecord?> GetSessionByIdAsync(string id);
    Task<bool> AddAsync(SessionRecord session);
    Task<bool> UpdateAsync(SessionRecord session);
}

public class SessionService(SessionDbContext db) : ISessionService
{
    public async Task<SessionRecord?> GetSessionByIdAsync(string id) =>
        await db.Sessions.FindAsync(id);

    public async Task<bool> AddAsync(SessionRecord session)
    {
        await db.Sessions.AddAsync(session);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(SessionRecord session)
    {
        var existing = await db.Sessions
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == session.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(session);
        await db.SaveChangesAsync();
        return true;
    }
}