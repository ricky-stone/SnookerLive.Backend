using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service(ILogger<Service> logger, IMessageBus bus, SeasonService season, IEventApiClient eventClient) : BackgroundService
{
    private const string QueueName = "snookerorg.high";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clock = new ShotClock(TimeSpan.FromMinutes(10), PublishRequestAsync);
        await clock.RunAsync(stoppingToken);
    }

    private async Task PublishRequestAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        try
        {
            var currentSeason = await season.GetCurrentSeasonAsync();
            logger.LogInformation("Fetching live events for current season {Season}", currentSeason);

            var events = await eventClient.GetAllLiveAsync(currentSeason);
            if (events is not { Count: > 0 })
            {
                logger.LogInformation("No live events found for season {Season}", currentSeason);
                return;
            }

            foreach(var @event in events)
            {
                if (stoppingToken.IsCancellationRequested)
                return;

                var message = new SnookerOrgMessage("Fetch Matches") { Url = ["t", "6", "e", @event.Id] };
                logger.LogInformation("Sending sync request to {Queue} (t=6&e={eventId})", QueueName, @event.Id);
                await bus.PublishAsync(QueueName, message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing upcoming event matches sync request");
        }
    }
}