using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service(ILogger<Service> logger, IMessageBus bus, IEventApiClient eventClient) : BackgroundService
{
    private const string QueueName = "snookerorg.low";
    private const int currentSeason = 2025;
    private const int targetSeason = 1950;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (int season = currentSeason; season >= targetSeason; season--)
        {
            if(stoppingToken.IsCancellationRequested)
                return;
            
            var eventsForSeason = await eventClient.GetAllForSeason(season);
            if (eventsForSeason is not { Count: > 0 })
            {
                logger.LogInformation("No events found for season {Season}", season);
                continue;
            }

            foreach(var @event in eventsForSeason)
            {
                if(stoppingToken.IsCancellationRequested)
                    return;

                var message = new SnookerOrgMessage("Fetch Matches") { Url = ["t", "6", "e", @event.Id] };
                logger.LogInformation("Sending sync request to {Queue} (t=6&e={eventId})", QueueName, @event.Id);
                await bus.PublishAsync(QueueName, message);
            }
        }
    }
}