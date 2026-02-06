using Domain;
using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public interface IRoundService
{
    Task<RoundRecord?> GetRoundByIdAsync(string id);
    Task<List<RoundRecord>> GetRoundsByEventIdAsync(int eventId);
    Task<bool> AddAsync(RoundRecord round);
    Task<bool> UpdateAsync(RoundRecord round);
}

public class RoundService(RoundDbContext db) : IRoundService
{

    public async Task<RoundRecord?> GetRoundByIdAsync(string id) =>
        await db.Rounds.FindAsync(id);

    public async Task<List<RoundRecord>> GetRoundsByEventIdAsync(int eventId) =>
        await db.Rounds
            .Where(r => r.EventID == eventId)
            .ToListAsync();

    public async Task<bool> AddAsync(RoundRecord round)
    {
        await db.Rounds.AddAsync(round);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(RoundRecord round)
    {
        var existing = await db.Rounds
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == round.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(round);
        await db.SaveChangesAsync();
        return true;
    }

}