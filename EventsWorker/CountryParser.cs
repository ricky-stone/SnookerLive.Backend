using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SnookerLive;

public static class CountryParser
{
    static readonly Dictionary<string, string> Map = new(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<string, string> Alias = new(StringComparer.OrdinalIgnoreCase);

    static CountryParser()
    {
        foreach (var f in typeof(CountryCode).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
        {
            if (f.FieldType != typeof(string)) continue;
            var code = (string?)f.GetRawConstantValue();
            var id = f.Name;
            var spaced = Regex.Replace(id, "(?<!^)([A-Z])", " $1");
            if (code != null) {
                AddKey(code, code);
                AddKey(id, code);
                AddKey(spaced, code);
                AddKey(spaced.Replace(" And ", " & "), code);
            }
        }

        Alias["uk"] = "GBR";
        Alias["u.k"] = "GBR";
        Alias["u.k."] = "GBR";
        Alias["gb"] = "GBR";
        Alias["greatbritain"] = "GBR";
        Alias["britain"] = "GBR";
        Alias["unitedkingdom"] = "GBR";
        Alias["unitedkingdomofgreatbritainandnorthernireland"] = "GBR";

        Alias["eng"] = "ENG";
        Alias["sco"] = "SCO";
        Alias["wal"] = "WAL";
        Alias["cymru"] = "WAL";
        Alias["nir"] = "GBR";
        Alias["n.ireland"] = "GBR";
        Alias["northern ireland"] = "GBR";
        Alias["northen ireland"] = "GBR";

        Alias["us"] = "USA";
        Alias["u.s"] = "USA";
        Alias["u.s."] = "USA";
        Alias["usa"] = "USA";
        Alias["unitedstates"] = "USA";
        Alias["unitedstatesofamerica"] = "USA";

        Alias["russia"] = "RUS";
        Alias["drc"] = "COD";
        Alias["democraticrepublicofthecongo"] = "COD";
        Alias["republicofthecongo"] = "COG";
        Alias["congobrazzaville"] = "COG";
        Alias["southkorea"] = "KOR";
        Alias["northkorea"] = "PRK";
        Alias["uae"] = "ARE";
        Alias["u.a.e"] = "ARE";
        Alias["ivorycoast"] = "CIV";
        Alias["czechrepublic"] = "CZE";
        Alias["swaziland"] = "SWZ";
        Alias["syria"] = "SYR";
        Alias["tanzania"] = "TZA";
        Alias["vatican"] = "VAT";
        Alias["macau"] = "MAC";
        Alias["laos"] = "LAO";
        Alias["taiwan"] = "TWN";
        Alias["venezuela"] = "VEN";
        Alias["bolivia"] = "BOL";
        Alias["capeverde"] = "CPV";
        Alias["hong kong"] = "HKG";
        Alias["moldova"] = "MDA";
        Alias["palestine"] = "PSE";
        Alias["reunion"] = "REU";
        Alias["cotedivoire"] = "CIV";
        Alias["cote d'ivoire"] = "CIV";
        Alias["côte d’ivoire"] = "CIV";
    }

    public static string ParseCountry(string input)
    {
        if (TryParseCountry(input, out var code)) return code;
        throw new ArgumentException($"Unrecognized country or code: {input}");
    }

    public static bool TryParseCountry(string input, out string alpha3)
    {
        alpha3 = null!;
        if (string.IsNullOrWhiteSpace(input)) return false;

        var norm = Normalize(input);

        if (Alias.TryGetValue(norm, out var aliasCode))
        {
            alpha3 = aliasCode;
            return true;
        }

        if (Map.TryGetValue(norm, out var mapped))
        {
            alpha3 = mapped;
            return true;
        }

        if (IsAlpha3(norm))
        {
            var upper = norm.ToUpperInvariant();
            if (Map.ContainsValue(upper) || upper is "ENG" or "SCO" or "WAL" or "GBR" or "IRL")
            {
                alpha3 = upper;
                return true;
            }
        }

        return false;
    }

    static void AddKey(string key, string code)
    {
        if (key == null || code == null) return;
        Map[Normalize(key)] = code;
    }

    static bool IsAlpha3(string s) => s.Length == 3 && s.All(char.IsLetter);

    static string Normalize(string s)
    {
        s = s.Trim().ToLowerInvariant();
        s = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(s.Length);
        foreach (var c in s)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat == UnicodeCategory.NonSpacingMark) continue;
            if (char.IsLetterOrDigit(c)) sb.Append(c);
            else if (c is ' ' or '&') sb.Append(c);
        }
        s = sb.ToString();
        s = Regex.Replace(s, @"\s+|[&]", "");
        return s;
    }
}