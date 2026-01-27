using System.Net.Http.Json;
using Domain;

namespace SessionWorker;

public interface ISessionApiClient
{
    Task<SessionRecord?> GetAsync(string id);
    Task<bool> AddAsync(SessionRecord session);
    Task<bool> UpdateAsync(SessionRecord session);
}

public sealed class SessionApiClient(HttpClient http) : ISessionApiClient
{
    public async Task<SessionRecord?> GetAsync(string id)
    {
        var response = await http.GetAsync(id);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get session {id}. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return await response.Content.ReadFromJsonAsync<SessionRecord>();
    }

    public async Task<bool> AddAsync(SessionRecord session)
    {
        var response = await http.PostAsJsonAsync(string.Empty, session);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get session. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return true;
    }

    public async Task<bool> UpdateAsync(SessionRecord session)
    {
        var response = await http.PutAsJsonAsync(string.Empty, session);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get session. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return true;
    }
}