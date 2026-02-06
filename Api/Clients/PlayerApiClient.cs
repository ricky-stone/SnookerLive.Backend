using Domain;

namespace SnookerLive;

public interface IPlayerApiClient
{
    Task<PlayerRecord?> GetAsync(string id);
}

public sealed class PlayerApiClient(HttpClient http) : IPlayerApiClient
{
    public async Task<PlayerRecord?> GetAsync(string id)
    {
        var response = await http.GetAsync(id);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get player {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<PlayerRecord>();
    }
}