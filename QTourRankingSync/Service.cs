using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service(ILogger<Service> logger, IMessageBus bus) : BackgroundService
{
    private const string RankingType = "QTRankings";
    private const string QueueName = "snookerorg.low";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clock = new ShotClock(TimeSpan.FromMinutes(60), PublishRequestAsync);
        await clock.RunAsync(stoppingToken);
    }

    private async Task PublishRequestAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        var message = new SnookerOrgMessage("Fetch Rankings") { Url = ["rt", RankingType] };

        logger.LogInformation("Sending sync request to {Queue} (t={T})", QueueName, RankingType);

        await bus.PublishAsync(QueueName, message);
    }
}