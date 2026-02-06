using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IRankingApiClient
{
    Task<RankingRecord?> GetAsync(string id);
    Task<bool> AddAsync(RankingRecord ranking);
    Task<bool> UpdateAsync(string id, RankingRecord ranking);
}

public sealed class RankingApiClient(HttpClient client) : IRankingApiClient
{
    public async Task<RankingRecord?> GetAsync(string id)
    {
        var response  = await client.GetAsync(id);
        if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to get ranking with id {id}: {response.StatusCode}, Body: {errorBody}");
        }
        
        return await response.Content.ReadFromJsonAsync<RankingRecord>();
    }

    public async Task<bool> AddAsync(RankingRecord ranking)
    {
        var response = await client.PostAsJsonAsync(string.Empty, ranking);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to add ranking: {response.StatusCode}, Body: {errorBody}");
        }

        return true;
    }

    public async Task<bool> UpdateAsync(string id, RankingRecord ranking)
    {
        var response = client.PutAsJsonAsync(string.Empty, ranking);
        if (!response.IsCompletedSuccessfully)
        {
            var errorBody = response.Result.Content.ReadAsStringAsync().Result;
            throw new Exception($"Failed to update ranking with id {id}: {response.Result.StatusCode}, Body: {errorBody}");
        }
        return true;
    }
}