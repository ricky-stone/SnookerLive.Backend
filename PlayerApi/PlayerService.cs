using Domain;
using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public interface IPlayerService
{
    Task<PlayerRecord?> GetPlayerByIdAsync(string id);
    Task<bool> AddAsync(PlayerRecord player);
    Task<bool> UpdateAsync(PlayerRecord player);
}

public class PlayerService(PlayerDbContext db) : IPlayerService
{

    public async Task<PlayerRecord?> GetPlayerByIdAsync(string id) =>
        await db.Players.FindAsync(id);

    public async Task<bool> AddAsync(PlayerRecord player)
    {
        await db.Players.AddAsync(player);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(PlayerRecord player)
    {
        var existing = await db.Players
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == player.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(player);
        await db.SaveChangesAsync();
        return true;
    }

}