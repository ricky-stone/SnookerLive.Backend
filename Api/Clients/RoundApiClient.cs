using Domain;

namespace SnookerLive;

public interface IRoundApiClient
{
    Task<RoundRecord?> GetAsync(string id);
    Task<List<RoundRecord>?> GetByEventIdAsync(string eventId);
}

public sealed class RoundApiClient(HttpClient http) : IRoundApiClient
{
    public async Task<RoundRecord?> GetAsync(string id)
    {
        var response = await http.GetAsync(id);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get round {id}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<RoundRecord>();
    }

    public async Task<List<RoundRecord>?> GetByEventIdAsync(string eventId)
    {
        var response = await http.GetAsync($"event/{eventId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"Failed to get rounds for event {eventId}. Response: {errorBody}",
                null,
                response.StatusCode);
        }

        var rounds = await response.Content.ReadFromJsonAsync<List<RoundRecord>>();
        if (rounds is null)
            return null;
        
        rounds = NormalizeFinalRound(rounds);

        return rounds;
    }

    private static List<RoundRecord> NormalizeFinalRound(List<RoundRecord> rounds)
    {
        if (rounds.Count == 0)
            return rounds;

        var loserFinal = rounds.FirstOrDefault(r =>
            r.Round == 15 &&
            string.Equals(r.RoundName, "Final", StringComparison.OrdinalIgnoreCase) &&
            r.NumLeft == 2);

        var winnerFinal = rounds.FirstOrDefault(r =>
            r.Round == 17 &&
            string.Equals(r.RoundName, "Final", StringComparison.OrdinalIgnoreCase) &&
            r.NumLeft == 1);

        if (loserFinal is null || winnerFinal is null)
            return rounds;

        loserFinal.WinnerMoney = winnerFinal.Money;

        rounds.Remove(winnerFinal);

        return rounds;
    }
}