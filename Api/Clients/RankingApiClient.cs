using Domain;

namespace SnookerLive;

public interface IRankingApiClient
{
    Task<List<RankingRecord>?> GetByValueTypeAndSeasonAsync(string type, int season);
}

public class RankingApiClient(HttpClient http) : IRankingApiClient
{
    public async Task<List<RankingRecord>?> GetByValueTypeAndSeasonAsync(string type, int season)
    {
        var response = await http.GetAsync($"{type}/{season}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get rankings for value type {type} and season {season}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<List<RankingRecord>>() ?? null;
    }
}