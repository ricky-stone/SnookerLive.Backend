using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class LiveMatchesSyncWorker(ILogger<LiveMatchesSyncWorker> logger, IMessageBus bus) : BackgroundService
{
    private const int LiveMatchesType = 17;
    private const string QueueName = "snookerorg.realtime";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clock = new ShotClock(TimeSpan.FromSeconds(45), PublishRequestAsync);
        await clock.RunAsync(stoppingToken);
    }

    private async Task PublishRequestAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        var message = new SnookerOrgMessage("Fetch Live Matches") { Url = ["t", LiveMatchesType] };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Sending sync request to {Queue} (t={T})", QueueName, LiveMatchesType);

        await bus.PublishAsync(QueueName, message);
    }
}