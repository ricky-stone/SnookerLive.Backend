namespace SnookerLive;

public sealed class Season
{
    public int CurrentSeason { get; set; }
}

public sealed class SeasonService
{
    public async Task<int> GetCurrentSeasonAsync()
    {
        var cachedSeason = Environment.GetEnvironmentVariable("CURRENT_SEASON");
        var lastSet = Environment.GetEnvironmentVariable("CURRENT_SEASON_TIMESTAMP");

        if (int.TryParse(cachedSeason, out var season) && DateTime.TryParse(lastSet, out var timestamp))
        {
            if ((DateTime.UtcNow - timestamp).TotalHours < 24)
            {
                return season;
            }
        }
    
        using var httpClient = new HttpClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.snooker.org/?t=20");
        request.Headers.Add("X-Requested-By", "RickyStone");
        var response = await httpClient.SendAsync(request);
        var message = response.EnsureSuccessStatusCode();
        if (!message.IsSuccessStatusCode)
        {
            throw new Exception("Failed to fetch current season from Snooker.org");
        }

        var content = await message.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(content) || content == "\"\"")
        {
            throw new Exception("Failed to fetch current season from Snooker.org");
        }

        var seasonData = System.Text.Json.JsonSerializer.Deserialize<List<Season>>(content);
        if (seasonData == null || seasonData.Count == 0)
        {
            throw new Exception("Failed to fetch current season from Snooker.org");
        }

        var currentSeason = seasonData[0].CurrentSeason;

        Environment.SetEnvironmentVariable("CURRENT_SEASON", currentSeason.ToString());
        Environment.SetEnvironmentVariable("CURRENT_SEASON_TIMESTAMP", DateTime.UtcNow.ToString());
        return currentSeason;
    }
}