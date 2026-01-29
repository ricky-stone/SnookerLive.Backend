using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace EventsSync;

public sealed class Service(ILogger<Service> logger, IMessageBus bus) : BackgroundService
{
    private const int EventType = 5;
    private const string QueueName = "snookerorg.high";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clock = new ShotClock(TimeSpan.FromMinutes(20), PublishRequestAsync);
        await clock.RunAsync(stoppingToken);
    }

    private async Task PublishRequestAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        var message = new SnookerOrgMessage("Fetch Events") { Url = ["t", EventType] };

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Sending sync request to {Queue} (t={T})", QueueName, EventType);

        await bus.PublishAsync(QueueName, message);
    }
}