using System.Globalization;
using Domain;

namespace SessionWorker;

public static class SessionBuilder
{
    static readonly TimeZoneInfo Oslo = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
    static readonly string[] WithTime = { "dd.MM.yyyy HH:mm", "dd.MM.yyyy HH:mm:ss" };
    static readonly string[] DateOnly = { "dd.MM.yyyy" };

    public static List<SessionRecord> BuildSessions(DateTime? scheduledStartUtc, string sessions, string matchId)
    {
        if (scheduledStartUtc == null && string.IsNullOrWhiteSpace(sessions))
            return new List<SessionRecord>();

        var list = new List<SessionRecord>();

        if (scheduledStartUtc is DateTime first)
            list.Add(new SessionRecord { MatchId = matchId, Utc = EnsureUtc(first), IsExplicitTime = true });

        if (!string.IsNullOrWhiteSpace(sessions))
        {
            foreach (var part in sessions.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var s = part.Trim();
                if (DateTime.TryParseExact(s, WithTime, CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out var dtWithTime))
                {
                    var local = DateTime.SpecifyKind(dtWithTime, DateTimeKind.Unspecified);
                    list.Add(new SessionRecord { MatchId = matchId, Utc = EnsureUtc(local), IsExplicitTime = true });
                }
                else if (DateTime.TryParseExact(s, DateOnly, CultureInfo.InvariantCulture, DateTimeStyles.None,
                             out var dtDateOnly))
                {
                    var local = DateTime.SpecifyKind(dtDateOnly.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                        DateTimeKind.Unspecified);
                    list.Add(new SessionRecord { MatchId = matchId, Utc = EnsureUtc(local), IsExplicitTime = false });
                }
            }
        }

        var ordered = list
            .GroupBy(x => x.Utc)
            .Select(g => g.First())
            .OrderBy(x => x.Utc)
            .ToList();

        var result = new List<SessionRecord>(ordered.Count);
        for (int i = 0; i < ordered.Count; i++)
        {
            var n = i + 1;
            var s = ordered[i];
            result.Add(new SessionRecord
            {
                Id = $"{matchId}-{n}",
                MatchId = matchId,
                SessionNumber = n,
                Utc = s.Utc,
                IsExplicitTime = s.IsExplicitTime
            });
        }

        return result;
    }

    static DateTime EnsureUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
}