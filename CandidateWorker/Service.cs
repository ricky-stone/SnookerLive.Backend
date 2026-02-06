using System.Reflection.Metadata;
using System.Text.Json;
using Comparer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rabbit;
using Redis;

namespace SnookerLive;

public sealed class Service(
    ILogger<Service> logger,
    IQueueConsumer<SnookerOrgDataResponse> queue,
    ICandidateApiService candidateApi,
    ICacheService redis) : BackgroundService
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
    
    private const string CandidateQueueName = "candidates";
    private const string CandidateCacheKeyPrefix = "candidates:";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        queue.Start(CandidateQueueName, HandleMessage);
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

        var candidates = JsonSerializer.Deserialize<List<SnookerOrgCandidateDto>>(rawData, JsonOptions);
        if (candidates is null)
        {
            logger.LogWarning("Failed to deserialize candidates data: {RawData}", rawData);
            return;
        }

        foreach (var candidate in candidates)
        {
            var record = SnookerOrgCandidateMapper.ToCandidate(candidate);
            await HandleIncomingCandidate(record);
        }
        
    }

    private async Task HandleIncomingCandidate(CandidateRecord candidate)
    {
        var exisitingCandidate = await candidateApi.GetAsync(candidate.Id);
        if (exisitingCandidate is null)
        {
            await candidateApi.AddAsync(candidate);
            await redis.SetAsync(CandidateCacheKeyPrefix + candidate.Id, candidate, TimeSpan.FromHours(1));
            return;
        }

        var differences = ModelComparer.Compare(exisitingCandidate, candidate);
        if (differences.Count == 0)
            return;
        
        await candidateApi.UpdateAsync(candidate);
        await redis.SetAsync(CandidateCacheKeyPrefix + candidate.Id, candidate, TimeSpan.FromHours(1));
    }
}