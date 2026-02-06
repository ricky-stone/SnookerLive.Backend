using Domain;

namespace SnookerLive;

public interface IEventApiClient
{
    Task<EventRecord?> GetAsync(string id);
}

public sealed class EventApiClient(HttpClient client) : IEventApiClient
{
    public async Task<EventRecord?> GetAsync(string id)
    {
        var response = await client.GetAsync(id);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get event {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<EventRecord>();
    }
}