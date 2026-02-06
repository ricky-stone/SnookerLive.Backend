using System.Net;
using System.Net.Http.Json;
using Domain;

namespace SnookerLive;

public interface IPlayerApiClient
{
    Task<PlayerRecord?> GetAsync(string id);
    Task<bool> AddAsync(PlayerRecord player);
    Task<bool> UpdateAsync(string id, PlayerRecord player);
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
            throw new PlayerApiException(
                $"Failed to get player {id}",
                response.StatusCode,
                errorBody);
        }

        return await response.Content.ReadFromJsonAsync<PlayerRecord>();
    }

    public async Task<bool> AddAsync(PlayerRecord player)
    {
        var response = await http.PostAsJsonAsync(string.Empty, player);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new PlayerApiException(
                $"Failed to add player {player.Id}",
                response.StatusCode,
                errorBody);
        }

        return true;
    }

    public async Task<bool> UpdateAsync(string id, PlayerRecord player)
    {
        var response = await http.PutAsJsonAsync(string.Empty, player);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new PlayerApiException(
                $"Failed to update player {id}",
                response.StatusCode,
                errorBody);
        }

        return true;
    }
}

public sealed class PlayerApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ResponseBody { get; }

    public PlayerApiException(string message, HttpStatusCode? statusCode = null, string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public PlayerApiException(string message, Exception innerException, HttpStatusCode? statusCode = null, string? responseBody = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}