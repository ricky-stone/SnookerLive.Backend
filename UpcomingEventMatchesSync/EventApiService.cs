using System.Net.Http.Json;
using Domain;

namespace UpcomingEventMatchesSync;

public interface IEventApiClient
{
    Task<List<EventRecord>?> GetAllUpcomingAsync(int season);
}

public sealed class EventApiClient(HttpClient client) : IEventApiClient
{

    public async Task<List<EventRecord>?> GetAllUpcomingAsync(int season)
    {
        var urlString = $"season/{season}/upcoming";

        var response = await client.GetAsync(urlString);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get event list for season {season} and nextDays {nextDays}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<List<EventRecord>>();
    }
}