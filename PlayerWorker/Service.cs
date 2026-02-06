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
    IPlayerApiClient playerClient,
    ICacheService redis) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

    private const string PlayerQueueName = "players";
    private const string PlayerCacheKeyPrefix = "player:";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(PlayerQueueName, HandleMessage);
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

        var players = JsonSerializer.Deserialize<List<SnookerOrgPlayerDto>>(rawData, JsonOptions);
        if (players is not { Count: > 0})
        {
            logger.LogWarning("No players returned from SnookerOrg response.");
            return;
        }

        foreach (var player in players)
        {
            var record = SnookerOrgPlayerMapper.ToPlayerRecord(player);
            await HandleIncomingPlayer(record);
        }
    }

    private async Task HandleIncomingPlayer(PlayerRecord player)
    {

        logger.LogInformation("Handling incoming player: {id} {Name}", player.Id, player.DisplayFullName);

        var exisitingPlayer = await playerClient.GetAsync(player.Id);
        if (exisitingPlayer is null)
        {
            await playerClient.AddAsync(player);
            await redis.SetAsync(PlayerCacheKeyPrefix + player.Id, player, TimeSpan.FromHours(24));
            return;
        }

        var differences = ModelComparer.Compare(exisitingPlayer, player);
        if (differences.Count == 0)
            return;
        
        await playerClient.UpdateAsync(player.Id, player);
        await redis.SetAsync(PlayerCacheKeyPrefix + player.Id, player, TimeSpan.FromHours(24));
    }
};