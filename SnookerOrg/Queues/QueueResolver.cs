namespace SnookerOrg.Queues;

public static class QueueResolver
{
    public static string? ResolveQueue(object?[] url)
    {
        if (url.Length == 0 || url.Length % 2 != 0)
            return null;

        var p = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < url.Length - 1; i += 2)
        {
            var key = url[i]?.ToString();
            if (string.IsNullOrWhiteSpace(key))
                continue;

            p[key.Trim()] = url[i + 1]?.ToString() ?? string.Empty;
        }

        if (p.ContainsKey("rt"))
            return "rankings";

        if (p.ContainsKey("p1") && p.ContainsKey("p2"))
            return "matches";

        if (p.ContainsKey("p"))
            return "players";

        if (p.ContainsKey("e") && p.ContainsKey("r") && p.ContainsKey("n"))
            return "matches";

        if (p.TryGetValue("t", out var tStr) && int.TryParse(tStr, out var t))
        {
            return t switch
            {
                5 => "events",
                6 or 7 or 8 or 14 or 15 or 17 or 19 => "matches",
                18 => "candidates",
                9 or 10 => "players",
                12 => "rounds",
                13 or 20 => "events",
                21 => "events",
                _ => null
            };
        }

        if (p.ContainsKey("e"))
            return "events";

        return null;
    }
}