using System.Net;
using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IMatchApiClient
{
    Task<MatchRecord?> GetAsync(string id);
    Task<bool> AddAsync(MatchRecord match);
    Task<bool> UpdateAsync(string id, MatchRecord match);
}

public sealed class MatchApiClient(HttpClient client) : IMatchApiClient
{
    public async Task<MatchRecord?> GetAsync(string id)
    {
        var response = await client.GetAsync(id);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<MatchRecord>();

        var errorBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Failed to get match {id}. Response: {errorBody}",
            null,
            response.StatusCode);
    }

    public async Task<bool> AddAsync(MatchRecord match)
    {
        var response = await client.PostAsJsonAsync(string.Empty, match);

        if (response.IsSuccessStatusCode)
            return true;

        var errorBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Failed to add match. Response: {errorBody}",
            null,
            response.StatusCode);
    }

    public async Task<bool> UpdateAsync(string id, MatchRecord match)
    {
        var response = await client.PutAsJsonAsync(string.Empty, match);

        if (response.IsSuccessStatusCode)
            return true;

        var errorBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"Failed to update match {id}. Response: {errorBody}",
            null,
            response.StatusCode);
    }
}