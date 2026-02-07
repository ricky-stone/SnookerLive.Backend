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

    public Task<SnookerEvent?> GetEventByIdAsync(string id)
    {
        return redis.GetOrSetAsync(
            $"event:full:{id}",
            TimeSpan.FromMinutes(19),
            async () =>
            {
                var evtTask = eventApiClient.GetAsync(id);
                var watchOnTask = watchOnApiClient.GetByEventIdAsync(id);
                var roundsTask = roundApiClient.GetByEventIdAsync(id);

                await Task.WhenAll(evtTask, watchOnTask, roundsTask);

                var evt = await evtTask;
                if (evt is null) return null;

                return new SnookerEvent
                {
                    Id = evt.Id,
                    Event = evt,
                    WatchOn = await watchOnTask,
                    Rounds = await roundsTask
                };
            });
    }

}