using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace SnookerLive;

public interface IFrameService
{
    Task<FrameRecord?> GetFrameByIdAsync(string id);
    Task<bool> AddAsync(FrameRecord frame);
    Task<bool> UpdateAsync(FrameRecord frame);
}

public class FrameService(FrameDbContext db) : IFrameService
{

    public async Task<FrameRecord?> GetFrameByIdAsync(string id) =>
        await db.Frames.FindAsync(id);
    public async Task<bool> AddAsync(FrameRecord frame)
    {
        await db.Frames.AddAsync(frame);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(FrameRecord frame)
    {
        var existing = await db.Frames
            .AsTracking()
            .FirstOrDefaultAsync(f => f.Id == frame.Id);
        if (existing is null) return false;

        db.Entry(existing).CurrentValues.SetValues(frame);
        await db.SaveChangesAsync();
        return true;
    }

}
