using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using SnookerOrg.Enums;
using SnookerOrg.Messages;
using SnookerOrg.Queues;

namespace SnookerOrg;

public sealed class SnookerOrgRequestWorker(
    QueueService queueService,
    IMessageBus bus,
    SnookerOrgApiDispatcher dispatcher,
    ILogger<SnookerOrgRequestWorker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            QueueMessage queueMessage;

            try
            {
                queueMessage = await queueService.Dequeue(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            await Handle(queueMessage.Message, queueMessage.Priority, stoppingToken);
        }
    }

    private async Task Handle(
        SnookerOrgMessage message,
        Priority priority,
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Received {Priority} message, currently there are {Count} message(s) in the queue",
            priority,
            queueService.Count);

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            var data = await dispatcher.EnqueueAsync(priority, cts.Token, message.Url);

            var queueName = QueueResolver.ResolveQueue(message.Url);

            if (queueName is null)
            {
                logger.LogWarning("Cannot resolve queue for Url. {url}", message.Url);
                return;
            }

            var responseMessage = new SnookerOrgDataResponse("IncomingData") { Data = data };

            await bus.PublishAsync(queueName, responseMessage);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed processing message)");
        }
    }
}