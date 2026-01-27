using Comparer;
using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;

namespace SessionWorker;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<SessionMessage> queue,
    ISessionApiClient sessionClient) : BackgroundService
{
    private const string SessionQueueName = "sessions";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(SessionQueueName, HandleMessage);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessage(SessionMessage message)
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

    private async Task HandleIncomingSession(SessionMessage data)
    {
        var sessions = SessionBuilder.BuildSessions(data.ScheduledStartDate, data.Sessions, data.MatchId);

        if (sessions.Count == 0)
            return;

        foreach (var session in sessions)
        {
            await ProcessMatchSession(session);
        }
    }

    private async Task ProcessMatchSession(SessionRecord session)
    {
        var exisitingSession = await sessionClient.GetAsync(session.Id);
        if (exisitingSession == null)
        {
            await sessionClient.AddAsync(session);
            return;
        }

        var differences = ModelComparer.Compare(exisitingSession, session);
        if (differences.Count == 0)
            return;

        await sessionClient.UpdateAsync(session);
    }
}