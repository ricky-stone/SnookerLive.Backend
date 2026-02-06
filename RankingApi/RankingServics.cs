using Domain;
using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public interface IRankingService
{
    Task<RankingRecord?> GetRankingByIdAsync(string id);
    Task<List<RankingRecord>> GetRankingsByTypeAndSeasonAsync(string type, int season, int page = 0, int pageSize = 0);
    Task<List<RankingRecord>> GetRankingsByValueTypeAndSeasonAsync(string type, int season, int page = 0, int pageSize = 0);
    Task<bool> AddAsync(RankingRecord ranking);
    Task<bool> UpdateAsync(RankingRecord ranking);
}

public class RankingService(ApplicationDbContext db) : IRankingService
{

    public async Task<RankingRecord?> GetRankingByIdAsync(string id) =>
        await db.Rankings.FindAsync(id);

    public async Task<List<RankingRecord>> GetRankingsByTypeAndSeasonAsync(string type, int season, int page = 0, int pageSize = 0)
    {
        if (page == 0 && pageSize == 0)
        {
            return await db.Rankings
                .Where(r => r.Type == type && r.Season == season)
                .ToListAsync();
        }
        return await db.Rankings
            .Where(r => r.Type == type && r.Season == season)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<RankingRecord>> GetRankingsByValueTypeAndSeasonAsync(string type, int season, int page = 0, int pageSize = 0)
    {
        if (page == 0 && pageSize == 0)
        {
            return await db.Rankings
                .Where(r => r.ValueType == type && r.Season == season)
                .ToListAsync();
        }
        return await db.Rankings
            .Where(r => r.ValueType == type && r.Season == season)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }


    public async Task<bool> AddAsync(RankingRecord ranking)
    {
        await db.Rankings.AddAsync(ranking);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(RankingRecord ranking)
    {
        var existing = await db.Rankings
            .AsTracking()
            .FirstOrDefaultAsync(s => s.Id == ranking.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(ranking);
        await db.SaveChangesAsync();
        return true;
    }

}