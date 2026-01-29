using System.Net.Http.Json;
using Domain;

namespace SessionWorker;

public interface ISessionApiClient
{
    Task<bool> UpsertAsync(SessionRecord session);
}

public sealed class SessionApiClient(HttpClient http) : ISessionApiClient
{
    public async Task<bool> UpsertAsync(SessionRecord session)
    {
        var response = await http.PutAsJsonAsync(session.Id, session);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to upsert session. Response: {errorBody}",
                null,
                response.StatusCode
            );
        }

        return true;
    }
}