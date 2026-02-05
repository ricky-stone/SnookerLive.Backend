using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service(ILogger<Service> logger, IMessageBus bus) : BackgroundService
{
    private const string QueueName = "snookerorg.low";
    private const int currentSeason = 2025;
    private const int targetSeason = 1950;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        for (var season = currentSeason; season >= targetSeason; season--)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var message = new SnookerOrgMessage("Fetch Events") { Url = ["t", "5", "s", season] };
            logger.LogInformation("Sending sync request to {Queue} (s={season})", QueueName, season);
            await bus.PublishAsync(QueueName, message);
        }

    }
}