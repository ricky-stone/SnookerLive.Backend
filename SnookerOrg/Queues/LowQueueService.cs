using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using SnookerOrg.Enums;
using SnookerOrg.Messages;

namespace SnookerOrg.Queues;

public sealed class LowQueueService: BackgroundService
{
    private readonly QueueService queueService;
    private readonly IQueueConsumer<SnookerOrgMessage> queue;
    private readonly ILogger logger;
    public LowQueueService(QueueService queueService, IQueueConsumer<SnookerOrgMessage> queue, ILogger<RealTimeQueueService> logger)
    {
        this.queueService = queueService;
        this.queue = queue;
        this.logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting consumer for snookerorg.low");
        queue.Start("snookerorg.low", OnMessage);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task OnMessage(SnookerOrgMessage message)
    {
        queueService.Enqueue(Priority.Low, message);
        return Task.CompletedTask;
    }
}