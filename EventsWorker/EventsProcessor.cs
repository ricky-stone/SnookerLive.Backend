using System.Text.Json;
using Comparer;
using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace SnookerLive;

public sealed class EventsProcessor(
    ILogger<EventsProcessor> logger,
    IQueueConsumer<SnookerOrgDataResponse> queue,
    IMessageBus bus,
    IEventApiClient client,
    ICacheService redis) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    
    private const string EventsQueueName = "events";
    private const string EventCacheKeyPrefix = "event:";
    private const string WatchOnQueueName = "watchon";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(EventsQueueName, HandleMessage);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessage(SnookerOrgDataResponse message)
    {
        try
        {
            switch (message.Action)
            {
                case "IncomingData":
                    await HandleIncomingData(message.Data);
                    break;
                default:
                    logger.LogWarning("Unknown Action: {Action}", message.Action);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message with Action: {Action}", message.Action);
        }
    }

    private async Task HandleIncomingData(string? rawData)
    {
        if (string.IsNullOrEmpty(rawData) || rawData == "\"\"")
        {
            logger.LogWarning("Received empty response from SnookerOrg.");
            return;
        }

        var events = JsonSerializer.Deserialize<List<SnookerOrgEventDto>>(rawData, JsonOptions);
        if (events is not { Count: > 0 })
        {
            logger.LogWarning("No events returned from SnookerOrg response.");
            return;
        }

        foreach (var @event in events)
        {
            var record = SnookerOrgEventMapper.ToEventDto(@event);
            await ProcessEvent(record);
        }
    }

    private async Task ProcessEvent(EventRecord @event)
    {

        if (!string.IsNullOrEmpty(@event.CommonNote))
        {
            var message = new WatchOnMessage(
                "IncomingData",
                @event.Id,
                @event.SnookerOrgId,
                @event.CommonNote
            );
            await bus.PublishAsync(WatchOnQueueName, message);
        }

        var exisitingEvent = await client.GetAsync(@event.Id);
        if (exisitingEvent is null)
        {
            await client.AddAsync(@event);
            await redis.SetAsync(
                EventCacheKeyPrefix + @event.Id,
                @event,
                TimeSpan.FromHours(8)
            );
            return;
        }

        var differences = ModelComparer.Compare(exisitingEvent, @event);
        if (differences.Count == 0)
            return;
        
        await client.UpdateAsync(@event);
        await redis.SetAsync(
            EventCacheKeyPrefix + @event.Id,
            @event,
            TimeSpan.FromHours(8)
        ); 
    }
}