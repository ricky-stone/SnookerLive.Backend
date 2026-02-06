using System.Text.Json;
using Comparer;
using Domain;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace SnookerLive;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<SnookerOrgDataResponse> queue,
    IRankingApiClient rankingClient,
    ICacheService redis) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    private const string RankingQueueName = "rankings";
    private const string RankingCacheKeyPrefix = "ranking:";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(RankingQueueName, HandleMessage);
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

        var rankings = JsonSerializer.Deserialize<List<SnookerOrgRankingDto>>(rawData, JsonOptions);
        if (rankings == null)
        {
            logger.LogError("Failed to deserialize ranking data: {RawData}", rawData);
            return;
        }

        var rankingTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MoneyRankings"] = "WST",
            ["WomensRankings"] = "WWS",
            ["MoneySeedings"] = "SEEDINGS",
            ["QTRankings"] = "QT",
            ["ProjectedMastersMoneySeedings"] = "MASTERS",
            ["ProjectedUKMoneySeedings"] = "UK",
            ["ProjectedWCMoneySeedings"] = "WC"
        };

        foreach (var ranking in rankings)
        {
            if (!rankingTypes.TryGetValue(ranking.Type ?? "", out var valueType))
            {
                logger.LogWarning("Unknown ranking type: {Type}", ranking.Type);
                continue;
            }

            var rankingRecord = SnookerOrgRankingMapper.ToRankingRecord(valueType, ranking);
            await HandleIncomingRanking(rankingRecord);
        }
    }

    private async Task HandleIncomingRanking(RankingRecord ranking)
    {
        var exisitingRanking = await rankingClient.GetAsync(ranking.Id);
        if (exisitingRanking is null)
        {
            await rankingClient.AddAsync(ranking);
            await redis.SetAsync(RankingCacheKeyPrefix + ranking.Id, ranking, TimeSpan.FromHours(24));
            return;
        }

        var differences = ModelComparer.Compare(exisitingRanking, ranking);
        if (differences.Count == 0)
            return;
        
        await rankingClient.UpdateAsync(ranking.Id, ranking);
        await redis.SetAsync(RankingCacheKeyPrefix + ranking.Id, ranking, TimeSpan.FromHours(24));
    }
}