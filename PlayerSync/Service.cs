using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SnookerLive;

public sealed class Service(Logger<Service> logger, IMessageBus bus) : BackgroundService
{
    public const int Endpoint = 10;
    private const string QueueName = "snookerorg.medium";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var clock = new ShotClock(TimeSpan.FromHours(8), PublishRequestAsync);
        await clock.RunAsync(stoppingToken);
    }

    private async Task PublishRequestAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
            return;

        var message = new SnookerOrgMessage("Fetch Players") { Url = ["t", Endpoint] };

        logger.LogInformation("Sending sync request to {Queue} (t={T})", QueueName, Endpoint);

        await bus.PublishAsync(QueueName, message);
    }
}