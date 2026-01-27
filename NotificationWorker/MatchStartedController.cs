using System.Text.Json;
using Domain;
using Microsoft.Extensions.Logging;

namespace NotificationWorker;

public sealed class MatchStartedController : IController
{
    private readonly ILogger<MatchStartedController> _logger;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

    public MatchStartedController(ILogger<MatchStartedController> logger)
    {
        Action = "MatchStarted";
        _logger = logger;
    }

    public override async Task HandleMessage(string message)
    {
        var match = JsonSerializer.Deserialize<MatchRecord>(message, JsonOptions)!;
        _logger.LogInformation(
            "Sending notification match started for {MatchId}: {Status}",
            match.Id, match.Status
        );
        await Task.CompletedTask;
    }

    public override bool Validate(string message)
    {
        try
        {
            var match = JsonSerializer.Deserialize<MatchRecord>(message, JsonOptions);
            return match != null;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning("JSON deserialization error: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}