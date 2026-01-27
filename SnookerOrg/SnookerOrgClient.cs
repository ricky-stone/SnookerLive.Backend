using Microsoft.Extensions.Logging;

namespace SnookerOrg;

public sealed class SnookerOrgClient(HttpClient http, ILogger<SnookerOrgClient> logger)
{
    #region SnookerOrg API Call Logic
    public async Task<HttpResponseMessage> Call(CancellationToken ct, params object?[] input)
    {
        var query = BuildQueryString(input);
        logger.LogInformation("Fetching from snooker.org, params: {query}", query);
        using var request = new HttpRequestMessage(HttpMethod.Get, "?" + query);
        return await http.SendAsync(request, ct);
    }

    private static string BuildQueryString(object?[] input)
    {
        var parts = new List<string>(input.Length / 2);

        for (int i = 0; i < input.Length; i += 2)
        {
            var paramName = input[i]?.ToString() ?? throw new ArgumentException("Invalid param name");
            var paramValue = input[i + 1]?.ToString() ?? string.Empty;
            parts.Add($"{Uri.EscapeDataString(paramName)}={Uri.EscapeDataString(paramValue)}");
        }

        return string.Join("&", parts);
    }

    #endregion
}