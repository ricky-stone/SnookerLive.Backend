using System.Globalization;
using Domain;
using Domain.Enums;

namespace SnookerLive;

public static class SnookerOrgEventMapper
{
    public static EventRecord ToEventDto(SnookerOrgEventDto evt)
    {
        var baseFormat = (EventFormat)evt.Format;
        var startDate = ParseDate(evt.StartDate, false);
        var endDate = ParseDate(evt.EndDate, true);

        if (IsShootOut(evt.ValueType, evt.Name))
            baseFormat = EventFormat.ShootOut;

        return new EventRecord
        {
            Id = IdGenerator.Generate(evt.Id),
            SnookerOrgId = evt.Id,
            Name = evt.Name,
            StartDate = startDate,
            EndDate = endDate,
            Sponsor = evt.Sponsor,
            Season = evt.Season,
            Type = ParseEventType(evt.Type),
            Num = evt.Num,
            Venue = evt.Venue,
            City = evt.City,
            Country = evt.Country,
            CountryCode = CountryParser.ParseCountry(evt.Country),
            Main = evt.Main,
            QualifyingEventId = 0,
            Sex = GenderParser.NormalizeGender(evt.Sex),
            Stage = ParseStage(evt.Stage),
            ShortName = evt.ShortName,
            RankingType = ParseRanking(evt.RankingType),
            Team = evt.Team,
            Format = baseFormat,
            AllRoundsAdded = evt.AllRoundsAdded,
            NumCompetitors = evt.NumCompetitors,
            NumUpcoming = evt.NumUpcoming,
            NumActive = evt.NumActive,
            NumResults = evt.NumResults,
            PrizeMoney = 0,
            HighBreakMoney = 0,
            Note = evt.Note,
            CommonNote = evt.CommonNote,
            PreviousEdition = evt.PreviousEdition,
            Tour = ParseTour(evt.Tour),
            DefendingChampionId = evt.DefendingChampion,
            TripleCrown = IsTripleCrown(evt.Name),
            HomeNations = IsHomeNations(evt.Name),
        };
    }

    #region Parsers (mapping-specific)

    private static DateTime ParseDate(string input, bool endOfDay)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty", nameof(input));

        var formats = new[] { "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "d/M/yyyy" };

        if (!DateTime.TryParseExact(
                input,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
            throw new FormatException($"Invalid date: {input}. Expected one of: yyyy-MM-dd, yyyy/MM/dd, dd/MM/yyyy, d/M/yyyy.");

        parsed = DateTime.SpecifyKind(parsed, DateTimeKind.Utc);

        var startOfDayUtc = new DateTime(parsed.Year, parsed.Month, parsed.Day, 0, 0, 0, DateTimeKind.Utc);
        return endOfDay ? startOfDayUtc.AddDays(1).AddTicks(-1) : startOfDayUtc;
    }

    private static EventType ParseEventType(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return EventType.Other;

        return input.Substring(0, 1).ToLower() switch
        {
            "r" => EventType.Ranking,
            "q" => EventType.Qualifying,
            "a" => EventType.Amateur,
            "i" => EventType.Invitational,
            "s" => EventType.Seniors,
            _ => EventType.Other
        };
    }

    private static Tour ParseTour(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Tour.Unknown;

        return input.Substring(0, 1).ToLower() switch
        {
            "m" => Tour.Main,
            "q" => Tour.QTour,
            "w" => Tour.Women,
            "s" => Tour.Seniors,
            _ => Tour.Unknown
        };
    }

    private static Stage ParseStage(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Stage.None;

        return input.Substring(0, 1).ToLower() switch
        {
            "q" => Stage.Qualifiers,
            "f" => Stage.Final,
            _ => Stage.None
        };
    }

    private static EventRankingType ParseRanking(string input) =>
        input.ToLower() switch
        {
            "qtr" => EventRankingType.QTourRanking,
            "wr" => EventRankingType.WorldRanking,
            "wwr" => EventRankingType.WomensWorldRanking,
            _ => EventRankingType.Unknown
        };

    #endregion

    #region Predicates

    private static bool IsLive(DateTime startDate, DateTime endDate)
    {
        var now = DateTime.UtcNow;
        return now >= startDate && now <= endDate.Date.AddDays(1).AddTicks(-1);
    }

    private static bool IsShootOut(string valueType, string eventName) =>
        string.Equals(valueType, "shootout", StringComparison.OrdinalIgnoreCase)
        || eventName.Contains("shoot", StringComparison.OrdinalIgnoreCase);

    private static bool IsHomeNations(string? eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName)) return false;

        return eventName.Contains("English Open", StringComparison.OrdinalIgnoreCase)
               || eventName.Contains("Northern Ireland Open", StringComparison.OrdinalIgnoreCase)
               || eventName.Contains("Scottish Open", StringComparison.OrdinalIgnoreCase)
               || eventName.Contains("Welsh Open", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTripleCrown(string? eventName)
    {
        if (string.IsNullOrWhiteSpace(eventName)) return false;

        return eventName.Equals("World Championship", StringComparison.OrdinalIgnoreCase)
               || eventName.Equals("UK Championship", StringComparison.OrdinalIgnoreCase)
               || eventName.Equals("Masters", StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}