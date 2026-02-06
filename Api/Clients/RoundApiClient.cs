using Domain;

namespace SnookerLive;

public interface IRoundApiClient
{
    Task<RoundRecord?> GetAsync(string id);
    Task<List<RoundRecord>?> GetByEventIdAsync(string eventId);
}

public sealed class RoundApiClient(HttpClient http) : IRoundApiClient
{
    public async Task<RoundRecord?> GetAsync(string id)
    {
        var response = await http.GetAsync(id);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get round {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<RoundRecord>();
    }

    public async Task<List<RoundRecord>?> GetByEventIdAsync(string eventId)
    {
        var response = await http.GetAsync($"event/{eventId}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get rounds for event {eventId}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<List<RoundRecord>>() ?? null;
    }   
}