using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using SnookerOrg.Enums;
using SnookerOrg.Messages;

namespace SnookerOrg.Queues;

public sealed class HighQueueService: BackgroundService
{
    private readonly QueueService queueService;
    private readonly IQueueConsumer<SnookerOrgMessage> queue;
    private readonly ILogger logger;
    public HighQueueService(QueueService queueService, IQueueConsumer<SnookerOrgMessage> queue, ILogger<HighQueueService> logger)
    {
        this.queueService = queueService;
        this.queue = queue;
        this.logger = logger;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start("snookerorg.high", OnMessage);
        return Task.CompletedTask;
    }

    private async Task OnMessage(SnookerOrgMessage message)
    {
        queueService.Enqueue(Priority.High, message);
    }
}