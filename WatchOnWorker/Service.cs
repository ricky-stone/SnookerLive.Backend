using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace SnookerLive;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<WatchOnMessage> queue,
    IWatchOnApiClient watchOnApiClient,
    ICacheService redis) : BackgroundService
{
    private const string WatchOnQueueName = "watchon";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(WatchOnQueueName, HandleMessage);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessage(WatchOnMessage message)
    {
        try
        {
            switch (message.Action)
            {
                case "IncomingData":
                    await HandleIncomingData(message);
                    break;
                default:
                    logger.LogWarning("Unknown Action: {Action}", message.Action);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle session message. Action: {Action}", message.Action);
        }
    }

    private async Task HandleIncomingData(WatchOnMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.WatchOnString))
        {
            logger.LogWarning("No WatchOnString provided in IncomingData message.");
            return;
        }

        var links = WatchOnParser.WatchOnLinks(message.WatchOnString);
        if (links.Count == 0)
        {
            logger.LogInformation("No valid WatchOn links found in IncomingData message.");
            return;
        }

        var watchOn = new WatchOn
        {
            Id = message.Id,
            SnookerOrgId = message.SnookerOrgId,
            Links = links
        };

        await watchOnApiClient.UpsertAsync(watchOn);
        await redis.SetAsync($"watchon:{watchOn.Id}", watchOn, TimeSpan.FromHours(2));
    }
}