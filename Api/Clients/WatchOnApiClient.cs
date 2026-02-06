using Domain;

namespace SnookerLive;

public interface IWatchOnApiClient
{
    Task<WatchOn?> GetAsync(string id);
    Task<List<WatchOn>?> GetByEventIdAsync(string eventId);
}

public sealed class WatchOnApiClient(HttpClient http) : IWatchOnApiClient
{
    public async Task<WatchOn?> GetAsync(string id)
    {
        var response = await http.GetAsync(id);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get watch on {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<WatchOn>();
    }

    public async Task<List<WatchOn>?> GetByEventIdAsync(string eventId)
    {
        var response = await http.GetAsync($"event/{eventId}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get watch on for event {eventId}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<List<WatchOn>>() ?? null;
    }   
}