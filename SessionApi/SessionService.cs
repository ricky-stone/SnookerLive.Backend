using Domain;
using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public interface ISessionService
{
    Task<SessionRecord?> GetSessionByIdAsync(string id);
    Task UpsertAsync(SessionRecord session);
}

public sealed class SessionService(SessionDbContext db) : ISessionService
{
    public Task<SessionRecord?> GetSessionByIdAsync(string id) =>
        db.Sessions.FindAsync(id).AsTask();

    public async Task UpsertAsync(SessionRecord session)
    {
        var existing = await db.Sessions.FindAsync(session.Id);

        if (existing is null)
            db.Sessions.Add(session);
        else
            db.Entry(existing).CurrentValues.SetValues(session);

        await db.SaveChangesAsync();
    }
}