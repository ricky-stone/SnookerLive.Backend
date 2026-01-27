using System.Text.Json;
using Domain;
using Microsoft.Extensions.Logging;

namespace NotificationWorker;

public sealed class MatchFinishedController : IController
{
    private readonly ILogger<MatchFinishedController> _logger;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

    public MatchFinishedController(ILogger<MatchFinishedController> logger)
    {
        Action = "MatchFinished";
        _logger = logger;
    }

    public override async Task HandleMessage(string message)
    {
        var match = JsonSerializer.Deserialize<MatchRecord>(message, JsonOptions)!;
        _logger.LogInformation(
            "Sending notification match finished for {MatchId}: {Status}",
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