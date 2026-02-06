using System.Net.Http.Json;

namespace SnookerLive;

public interface ICandidateApiService
{
    Task<CandidateRecord?> GetAsync(string id);
    Task<bool> AddAsync(CandidateRecord candidate);
    Task<bool> UpdateAsync(CandidateRecord candidate);
}

public sealed class CandidateApiService(HttpClient client) : ICandidateApiService
{
    public async Task<CandidateRecord?> GetAsync(string id)
    {
        var response = await client.GetAsync(id);

        if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get match {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<CandidateRecord>();
    }

    public async Task<bool> AddAsync(CandidateRecord candidate)
    {
        var response = await client.PostAsJsonAsync(string.Empty, candidate);

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

    public async Task<bool> UpdateAsync(CandidateRecord candidate)
    {
        var response = await client.PutAsJsonAsync(string.Empty, candidate);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to update match {candidate.Id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }
        return true;
    }
}