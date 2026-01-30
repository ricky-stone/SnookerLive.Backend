using System.Net.Http.Json;
using Domain;

namespace EventsWorker;

public interface IEventApiClient
{
    Task<EventRecord?> GetAsync(string id);
    Task<bool> AddAsync(EventRecord evt);
    Task<bool> UpdateAsync(EventRecord evt);
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
                $"Failed to get match {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<EventRecord>();
    }

    public async Task<bool> AddAsync(EventRecord evt)
    {
        var response = await client.PostAsJsonAsync(string.Empty, evt);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to add match. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return true;
    }
    
    public async Task<bool> UpdateAsync(EventRecord evt)
    {
        var response = await client.PutAsJsonAsync(string.Empty, evt);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to update match {evt.Id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return true;
    }

}