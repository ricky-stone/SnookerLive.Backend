using System.Diagnostics.CodeAnalysis;
using Comparer;
using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace FrameWorker;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<FrameMessage> queue,
    IFrameApiClient frameApiClient,
    ICacheService redis) : BackgroundService
{

    public const string FrameQueueName = "frames";
    public const string FrameCacheKeyPrefix = "frame:";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(FrameQueueName, HandleMessage);
    }

    private async Task HandleMessage(FrameMessage message)
    {
        switch (message.Action)
        {
            case "IncomingData":
                await HandleIncomingFrame(message);
                break;
            default:
                logger.LogWarning("Unknown action: {Action}", message.Action);
                break;
        }
    }

    private async Task HandleIncomingFrame(FrameMessage data)
    {
        var frames = FrameScoreBuilder.BuildFrameScores(data.Match);
        if (frames.Count == 0)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("No frames to process for match {MatchId}", data.Match.Id);
            return;
        }

        foreach (var frame in frames)
            await ProcessFrame(frame);
    }

    private async Task ProcessFrame(FrameRecord frame)
    {
        var exsisitingFrame = await frameApiClient.GetAsync(frame.Id);
        if (exsisitingFrame is null)
        {
            await frameApiClient.AddAsync(frame);
            await redis.SetAsync(
                FrameCacheKeyPrefix + frame.Id,
                frame,
                TimeSpan.FromHours(2)
            );
            return;
        }

        var differences = ModelComparer.Compare(frame, exsisitingFrame);
        if (differences.Count == 0)
            return;

        await frameApiClient.UpdateAsync(frame);
        await redis.SetAsync(
            FrameCacheKeyPrefix + frame.Id,
            frame,
            TimeSpan.FromHours(2)
        );
    }
}