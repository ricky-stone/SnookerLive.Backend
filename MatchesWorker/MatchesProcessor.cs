using System.Text.Json;
using Comparer;
using Domain;
using Domain.Enums;
using MatchesWorker.Dto;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace MatchesWorker;

public sealed class MatchesProcessor(
    ILogger<MatchesProcessor> logger,
    IQueueConsumer<SnookerOrgDataResponse> queue,
    IMatchApiClient client,
    IMessageBus bus,
    ICacheService redis) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    private const string MatchesQueueName = "matches";
    private const string NotificationsQueueName = "notifications";
    private const string SessionsQueueName = "sessions";
    private const string FramesQueueName = "frames";
    private const string MatchCacheKeyPrefix = "match:";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(MatchesQueueName, HandleMessage);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessage(SnookerOrgDataResponse message)
    {
        switch (message.Action)
        {
            case "IncomingData":
                await HandleIncomingData(message.Data);
                break;
            default:
                logger.LogWarning("Unknown Action: {Action}", message.Action);
                break;
        }
    }

    private async Task HandleIncomingData(string? rawData)
    {
        if (string.IsNullOrEmpty(rawData) || rawData == "\"\"")
        {
            logger.LogWarning("Received empty response from SnookerOrg.");
            return;
        }

        var matches = JsonSerializer.Deserialize<List<SnookerOrgMatchDto>>(rawData, JsonOptions);
        if (matches is not { Count: > 0 })
        {
            logger.LogWarning("No matches returned from SnookerOrg response.");
            return;
        }

        foreach (var match in matches)
        {
            var matchRecord = SnookerOrgMatchMapper.ToMatchDto(match);
            await Task.WhenAll(
                ProcessMatch(matchRecord),
                ProcessMatchSessions(matchRecord),
                ProcessMatchFrames(matchRecord)
            );
        }
    }

    private async Task ProcessMatchFrames(MatchRecord match)
    {
        if(match.FrameScores == "")
        {
            logger.LogInformation("No frame scores to process for match {MatchId}, skipping", match.Id);
            return;
        }

        var message = new FrameMessage("IncomingData", match);
        await bus.PublishAsync(FramesQueueName, message);
    }

    private async Task ProcessMatchSessions(MatchRecord match)
    {
        var message = new SessionMessage("IncomingData")
        {
            ScheduledStartDate = match.ScheduledDate,
            Sessions = match.Sessions,
            MatchId = match.Id
        };

        await bus.PublishAsync(SessionsQueueName, message);
    }

    private async Task ProcessMatch(MatchRecord match)
    {
        var existingMatch = await client.GetAsync(match.Id);
        if (existingMatch is null)
        {
            await client.AddAsync(match);
            await redis.SetAsync(MatchCacheKeyPrefix + match.Id, match, TimeSpan.FromHours(2));
            return;
        }

        var differences = ModelComparer.Compare(existingMatch, match);
        if (differences.Count == 0)
            return;

        var hasStatusChanged = false;
        var hasScoreChanged = false;

        await client.UpdateAsync(match.Id, match);
        await redis.SetAsync(MatchCacheKeyPrefix + match.Id, match, TimeSpan.FromHours(2));

        foreach (var diff in differences)
        {
            switch (diff.PropertyName)
            {
                case nameof(MatchRecord.Status):
                    hasStatusChanged = true;
                    break;
                case nameof(MatchRecord.Score1):
                case nameof(MatchRecord.Score2):
                    hasScoreChanged = true;
                    break;
            }
        }

        if (hasScoreChanged)
            await HandleScoreChange(match);

        if (hasStatusChanged)
            await HandleMatchStatusChange(existingMatch, match);
    }

    private async Task HandleMatchStatusChange(MatchRecord oldMatch, MatchRecord newMatch)
    {
        var message = (oldMatch.Status, newMatch.Status) switch
        {
            (MatchStatus.Upcoming, MatchStatus.Live) =>
                CreateMatchNotificationMessage("MatchStarted", newMatch),

            (MatchStatus.Upcoming, MatchStatus.Ongoing) =>
                CreateMatchNotificationMessage("MatchInbetweenSessions", newMatch),

            (MatchStatus.Upcoming, MatchStatus.Finished) =>
                CreateMatchNotificationMessage("MatchFinished", newMatch),

            (MatchStatus.Live, MatchStatus.Ongoing) =>
                CreateMatchNotificationMessage("MatchInbetweenSessions", newMatch),

            (MatchStatus.Live, MatchStatus.Finished) =>
                CreateMatchNotificationMessage("MatchFinished", newMatch),

            (MatchStatus.Live, MatchStatus.Upcoming) =>
                CreateMatchStringNotificationMessage(
                    "MatchStatusReverted",
                    RevertedText(newMatch.Status),
                    newMatch
                ),

            (MatchStatus.Ongoing, MatchStatus.Live) =>
                CreateMatchNotificationMessage("MatchResumed", newMatch),

            (MatchStatus.Ongoing, MatchStatus.Finished) =>
                CreateMatchNotificationMessage("MatchFinished", newMatch),

            (MatchStatus.Ongoing, MatchStatus.Upcoming) =>
                CreateMatchStringNotificationMessage(
                    "MatchStatusReverted",
                    RevertedText(newMatch.Status),
                    newMatch
                ),

            (MatchStatus.Finished, MatchStatus.Upcoming) =>
                CreateMatchStringNotificationMessage(
                    "MatchStatusReverted",
                    RevertedText(newMatch.Status),
                    newMatch
                ),

            (MatchStatus.Finished, MatchStatus.Live) =>
                CreateMatchStringNotificationMessage(
                    "MatchStatusReverted",
                    RevertedText(newMatch.Status),
                    newMatch
                ),

            (MatchStatus.Finished, MatchStatus.Ongoing) =>
                CreateMatchStringNotificationMessage(
                    "MatchStatusReverted",
                    RevertedText(newMatch.Status),
                    newMatch
                ),

            _ => null
        };

        if (message is null)
        {
            logger.LogWarning(
                "Unhandled Match Status change: {oldStatus} -> {newStatus}",
                oldMatch.Status,
                newMatch.Status
            );
            return;
        }

        await bus.PublishAsync("notifications", message);
    }

    private async Task HandleScoreChange(MatchRecord newMatch)
    {
        var message = CreateMatchNotificationMessage("MatchScoreChanged", newMatch);
        await bus.PublishAsync(NotificationsQueueName, message);
    }

    #region Notification Helpers

    private NotificationMessage CreateMatchStringNotificationMessage(string action, string text, MatchRecord match)
    {
        var message = new
        {
            Text = text,
            Match = match
        };

        return new NotificationMessage(action)
        {
            Data = JsonSerializer.Serialize(message, JsonOptions)
        };
    }

    private NotificationMessage CreateMatchNotificationMessage(string action, MatchRecord match)
    {
        return new NotificationMessage(action)
        {
            Data = JsonSerializer.Serialize(match, JsonOptions)
        };
    }

    private static string RevertedText(MatchStatus status) => status switch
    {
        MatchStatus.Upcoming => "Match has been reverted to Upcoming status",
        MatchStatus.Live => "Match has been reverted to Live status",
        MatchStatus.Ongoing => "Match has been reverted to Inbetween Sessions status",
        MatchStatus.Finished => "Match has been reverted to Finished status",
        _ => "Match status has been reverted"
    };

    #endregion
}