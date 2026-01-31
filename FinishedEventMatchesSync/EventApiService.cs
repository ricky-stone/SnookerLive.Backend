using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IEventApiClient
{
    Task<List<EventRecord>?> GetAllFinishedAsync(int season, int lastDays);
}

public sealed class EventApiClient(HttpClient client) : IEventApiClient
{

    public async Task<List<EventRecord>?> GetAllFinishedAsync(int season, int lastDays)
    {
        var urlString = $"season/{season}/finished?lastDays={lastDays}";

        var response = await client.GetAsync(urlString);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get event list for season {season}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<List<EventRecord>>();
    }
}