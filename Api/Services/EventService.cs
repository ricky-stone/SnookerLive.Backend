using Redis;

namespace SnookerLive;

public interface IEventService
{
    Task<SnookerEvent?> GetEventByIdAsync(string id);
}

public sealed class EventService(
    IEventApiClient eventApiClient,
    IWatchOnApiClient watchOnApiClient,
    IRoundApiClient roundApiClient,
    ICacheService redis) : IEventService
{

    public async Task<SnookerEvent?> GetEventByIdAsync(string id)
    {

        var evtTask = redis.GetOrSetAsync(
            $"event:{id}",
            TimeSpan.FromHours(2),
            () => eventApiClient.GetAsync(id));

        var watchOnTask = redis.GetOrSetAsync(
            $"watchon:{id}",
            TimeSpan.FromHours(2),
            () => watchOnApiClient.GetByEventIdAsync(id));
        
        var roundsTask = redis.GetOrSetAsync(
            $"rounds:{id}",
            TimeSpan.FromHours(2),
            () => roundApiClient.GetByEventIdAsync(id));
        

        await Task.WhenAll(evtTask, watchOnTask, roundsTask);

        if(evtTask.Result == null)
            return null;

        return new SnookerEvent
        {
            Id = evtTask.Result.Id,
            Event = evtTask.Result,
            WatchOn = watchOnTask.Result,
            Rounds = roundsTask.Result
        };
    }

}