using System.Net;
using System.Text.RegularExpressions;

namespace SnookerLive;

public sealed record Link(string Name, string? Url);

public static class WatchOnParser
{
    public static List<Link> WatchOnLinks(string text)
    {
        var tuples = WatchOnTuples(text);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var list = new List<Link>();

        for (int i = 0; i < tuples.Length; i++)
        {
            var (name, link) = tuples[i];

            if (link == null && i + 1 < tuples.Length && !string.IsNullOrEmpty(tuples[i + 1].Link))
                link = tuples[i + 1].Link;

            if (seen.Add(name))
                list.Add(new Link(name, link));
        }

        return list;
    }

    private static (string Name, string Link)[] WatchOnTuples(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return Array.Empty<(string, string)>();

        var results = new List<(string, string)>();
        var cleaned = Regex.Replace(text, "<!--.*?-->", "", RegexOptions.Singleline);

        var anchorRegex = new Regex(@"<a\s+[^>]*href\s*=\s*[""']([^""']+)[""'][^>]*>(.*?)</a>", RegexOptions.IgnoreCase);

        int lastIndex = 0;
        string currentBasePrefix = null;

        foreach (Match m in anchorRegex.Matches(cleaned))
        {
            if (m.Index > lastIndex)
            {
                var beforeRaw = cleaned[lastIndex..m.Index];
                var openIdx = beforeRaw.LastIndexOf('(');
                var closeIdx = beforeRaw.LastIndexOf(')');
                if (openIdx >= 0 && (closeIdx < 0 || closeIdx < openIdx))
                {
                    var prefix = beforeRaw[..openIdx];
                    currentBasePrefix = Normalize(prefix);
                }
                else
                {
                    var before = Normalize(beforeRaw).Trim(' ', ',', '&', '/');
                    if (!string.IsNullOrWhiteSpace(before))
                        results.Add((before, null));
                }
            }

            var href = m.Groups[1].Value;
            var label = WebUtility.HtmlDecode(StripTags(m.Groups[2].Value)).Trim();

            foreach (var split in SplitComposite(label))
            {
                var name = split;
                if (!string.IsNullOrEmpty(currentBasePrefix))
                    name = Normalize($"{currentBasePrefix} {split}");
                results.Add((name, href));
            }

            lastIndex = m.Index + m.Length;

            if (m.NextMatch().Success)
            {
                var nextStart = m.NextMatch().Index;
                if (nextStart > lastIndex)
                {
                    var between = cleaned[lastIndex..nextStart];
                    if (between.Contains(')'))
                    {
                        var idx = between.IndexOf(')');
                        lastIndex += idx + 1;
                        currentBasePrefix = null;
                    }
                }
            }
        }

        if (lastIndex < cleaned.Length)
        {
            var tail = cleaned[lastIndex..];
            var close = tail.IndexOf(')');
            if (close >= 0)
            {
                tail = tail[(close + 1)..];
                currentBasePrefix = null;
            }

            var after = Normalize(tail).Trim(' ', ',', '&', '/');
            if (!string.IsNullOrWhiteSpace(after))
                results.Add((after, null));
        }

        return results.ToArray();
    }

    private static string Normalize(string input)
    {
        input = Regex.Replace(input ?? "", @"\s+", " ").Trim();
        input = input.TrimEnd('/');
        return input;
    }

    private static string StripTags(string input) => Regex.Replace(input ?? "", "<.*?>", "");

    private static IEnumerable<string> SplitComposite(string label)
    {
        var match = Regex.Match(label, @"^(.*?)\((.*?)\)$");
        if (match.Success)
        {
            var baseName = match.Groups[1].Value.Trim();
            var inner = match.Groups[2].Value;
            var parts = inner.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var p in parts)
                yield return $"{baseName} {p.Trim()}";
            yield break;
        }
        yield return label;
    }
}