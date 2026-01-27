using Domain;
using Microsoft.EntityFrameworkCore;

namespace MatchApi;

public interface IMatchService
{
    Task<MatchRecord?> GetMatchByIdAsync(string id);
    Task<bool> AddAsync(MatchRecord match);
    Task<bool> UpdateAsync(MatchRecord match);
}

public class MatchService(MatchDbContext db) : IMatchService
{
    public async Task<MatchRecord?> GetMatchByIdAsync(string id) =>
        await db.Matches.FindAsync(id);

    public async Task<bool> AddAsync(MatchRecord match)
    {
        await db.Matches.AddAsync(match);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(MatchRecord match)
    {
        var existing = await db.Matches
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == match.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(match);
        await db.SaveChangesAsync();
        return true;
    }
}