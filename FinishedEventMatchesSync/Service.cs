using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service(ILogger<Service> logger, IMessageBus bus, SeasonService season, IEventApiClient eventClient) : BackgroundService
{
    private const string QueueName = "snookerorg.low";
    private const int LastDays = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clock = new ShotClock(TimeSpan.FromHours(24), PublishRequestAsync);
        await clock.RunAsync(stoppingToken);
    }

    private async Task PublishRequestAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        try
        {
            var currentSeason = await season.GetCurrentSeasonAsync();
            logger.LogInformation("Fetching finished events for current season {Season}", currentSeason);

            var events = await eventClient.GetAllFinishedAsync(currentSeason, LastDays);
            if (events is not { Count: > 0 })
            {
                logger.LogInformation("No finished events found for season {Season} with lastDays={LastDays}", currentSeason, LastDays);
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