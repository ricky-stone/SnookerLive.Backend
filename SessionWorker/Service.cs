using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace SessionWorker;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<SessionMessage> queue,
    ISessionApiClient sessionClient,
    ICacheService redis) : BackgroundService
{
    private const string SessionQueueName = "sessions";
    private const string SessionCacheKeyPrefix = "session:";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(SessionQueueName, HandleMessage);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessage(SessionMessage message)
    {
        try
        {
            switch (message.Action)
            {
                case "IncomingData":
                    await HandleIncomingSession(message);
                    break;
                default:
                    logger.LogWarning("Unknown Action: {Action}", message.Action);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle session message. Action: {Action}", message.Action);
            throw;
        }
    }

    private async Task HandleIncomingSession(SessionMessage data)
    {
        var sessions = SessionBuilder.BuildSessions(
            data.ScheduledStartDate,
            data.Sessions,
            data.MatchId
        );

        if (sessions.Count == 0)
            return;

        foreach (var session in sessions)
            await ProcessMatchSession(session);
    }

    private async Task ProcessMatchSession(SessionRecord session)
    {
        if (string.IsNullOrWhiteSpace(session.Id))
        {
            logger.LogWarning("SessionRecord missing Id. Skipping.");
            return;
        }

        await sessionClient.UpsertAsync(session);
        await redis.SetAsync(
            SessionCacheKeyPrefix + session.Id,
            session,
            TimeSpan.FromHours(2)
        );
    }
}