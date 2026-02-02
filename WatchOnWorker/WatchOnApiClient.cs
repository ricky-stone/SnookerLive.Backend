using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IWatchOnApiClient
{
    Task<bool> UpsertAsync(WatchOn watchOn);
}

public sealed class WatchOnApiClient(HttpClient http) : IWatchOnApiClient
{
    public async Task<bool> UpsertAsync(WatchOn watchOn)
    {
        var response = await http.PutAsJsonAsync(watchOn.Id, watchOn);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to upsert watchon. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return true;
    }
}