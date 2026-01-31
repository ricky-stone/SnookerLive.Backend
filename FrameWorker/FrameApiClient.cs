using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IFrameApiClient
{
    Task<FrameRecord?> GetAsync(string id);
    Task<bool> AddAsync(FrameRecord frame);
    Task<bool> UpdateAsync(FrameRecord frame);
}

public sealed class FrameApiClient(HttpClient http) : IFrameApiClient
{
    public async Task<FrameRecord?> GetAsync(string id)
    {
        var response = await http.GetAsync(id);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get frame {id}. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return await response.Content.ReadFromJsonAsync<FrameRecord>();
    }

    public async Task<bool> AddAsync(FrameRecord frame)
    {
        var response = await http.PostAsJsonAsync(string.Empty, frame);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get frame. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return true;
    }

    public async Task<bool> UpdateAsync(FrameRecord frame)
    {
        var response = await http.PutAsJsonAsync(string.Empty, frame);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get frame. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return true;
    }
}