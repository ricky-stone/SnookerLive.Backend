using System.Text.Json;
using Comparer;
using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Rabbit;
using Redis;

namespace SnookerLive;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<SnookerOrgDataResponse> queue,
    IRoundApiClient client,
    ICacheService redis) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    private const string RoundQueueName = "rounds";
    private const string RoundCacheKeyPrefix = "rounds:";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(RoundQueueName, HandleMessage);
        return Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleMessage(SnookerOrgDataResponse message)
    {
        try
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing message with Action: {Action}", message.Action);
        }
    }

    private async Task HandleIncomingData(string? rawData)
    {
        if (string.IsNullOrEmpty(rawData) || rawData == "\"\"")
        {
            logger.LogWarning("Received empty response from SnookerOrg.");
            return;
        }

        var rounds = JsonSerializer.Deserialize<List<SnookerOrgRoundInfoDto>>(rawData, JsonOptions);
        if (rounds is null)
        {
            logger.LogWarning("Failed to deserialize SnookerOrg data: {RawData}", rawData);
            return;
        }

        foreach (var round in rounds)
        {
            var record = SnookerOrgRoundMapper.ToRecord(round);
            await HandleIncomingRound(record);
        }
    }

    private async Task HandleIncomingRound(RoundRecord round)
    {
        var exisitingRound = await client.GetAsync(round.Id);
        if (exisitingRound is null)
        {
            await client.AddAsync(round);
            await redis.SetAsync(RoundCacheKeyPrefix + round.Id, round, TimeSpan.FromHours(2));
            return;
        }

        var diffrences = ModelComparer.Compare(round, exisitingRound);
        if (diffrences.Count == 0)
            return;
        
        await client.UpdateAsync(round.Id, round);
        await redis.SetAsync(RoundCacheKeyPrefix + round.Id, round, TimeSpan.FromHours(2));
    }
};