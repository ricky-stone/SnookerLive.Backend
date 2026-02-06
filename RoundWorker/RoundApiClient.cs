using System.Net;
using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IRoundApiClient
{
    Task<RoundRecord?> GetAsync(string id);
    Task<bool> AddAsync(RoundRecord round);
    Task<bool> UpdateAsync(string id, RoundRecord round);
}

public sealed class RoundApiClient(HttpClient client) : IRoundApiClient
{

    public async Task<RoundRecord?> GetAsync(string id)
    {
        var response = await client.GetAsync(id);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new RoundApiException(
                $"Failed to get round {id}",
                response.StatusCode,
                errorBody);
        }

        return await response.Content.ReadFromJsonAsync<RoundRecord>();
    }

    public async Task<bool> AddAsync(RoundRecord round)
    {
        var response = await client.PostAsJsonAsync(string.Empty, round);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new RoundApiException(
                $"Failed to add round {round.Id}",
                response.StatusCode,
                errorBody);
        }

        return true;
    }
    
    public async Task<bool> UpdateAsync(string id, RoundRecord round)
    {
        var response = await client.PutAsJsonAsync(string.Empty, round);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new RoundApiException(
                $"Failed to update round {id}",
                response.StatusCode,
                errorBody);
        }

        return true;
    }

}

public sealed class RoundApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ResponseBody { get; }

    public RoundApiException(string message, HttpStatusCode? statusCode = null, string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public RoundApiException(string message, Exception innerException, HttpStatusCode? statusCode = null, string? responseBody = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}