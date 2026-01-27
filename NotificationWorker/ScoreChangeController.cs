using System.Text.Json;
using Domain;
using Microsoft.Extensions.Logging;

namespace NotificationWorker;

public sealed class ScoreChangeController : IController
{
    private readonly ILogger<ScoreChangeController> _logger;

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

    public ScoreChangeController(ILogger<ScoreChangeController> logger)
    {
        Action = "MatchScoreChanged";
        _logger = logger;
    }

    public override async Task HandleMessage(string message)
    {
        var match = JsonSerializer.Deserialize<MatchRecord>(message, JsonOptions)!;
        _logger.LogInformation(
            "Sending notification for match {MatchId}: {Score1}-{Score2}",
            match.Id, match.Score1, match.Score2
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