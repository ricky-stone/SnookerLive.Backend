using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Domain;
using Domain.Enums;

namespace SnookerLive;

public static class SnookerOrgMatchMapper
{
    public static MatchRecord ToMatchDto(SnookerOrgMatchDto match) => new()
    {
        Id = BuildId(match.EventId, match.Round, match.Number),

        EventId = match.EventId,
        Round = match.Round,
        Number = match.Number,

        Player1Id = match.Player1Id,
        Score1 = match.Score1,
        Walkover1 = match.Walkover1,

        Player2Id = match.Player2Id,
        Score2 = match.Score2,
        Walkover2 = match.Walkover2,

        WinnerId = match.WinnerId,
        Unfinished = match.Unfinished,
        OnBreak = match.OnBreak,

        Status = ToMatchStatus(match.Status),
        WorldSnookerId = match.WorldSnookerId,

        LiveUrl = match.LiveUrl ?? string.Empty,
        DetailsUrl = match.DetailsUrl ?? string.Empty,

        PointsDropped = match.PointsDropped,
        ShowCommonNote = match.ShowCommonNote,
        Estimated = match.Estimated,

        Type = ToMatchKind(match.Type),
        TableNo = match.TableNo,
        VideoUrl = match.VideoUrl ?? string.Empty,

        InitDate = ParseZuluUtc(match.InitDate),
        ModDate = ParseZuluUtc(match.ModDate),
        StartDate = ParseZuluUtc(match.StartDate),
        EndDate = ParseZuluUtc(match.EndDate),
        ScheduledDate = ParseZuluUtc(match.ScheduledDate),

        Distance = 0,
        MidSessionTimeStamp = null,

        Note = StripHtml(match.Note),
        ExtendedNote = StripHtml(match.ExtendedNote),

        HeldOver = match.HeldOver,
        StatsUrl = match.StatsUrl ?? string.Empty,

        FrameScores = match.FrameScores ?? string.Empty,
        Sessions = match.Sessions ?? string.Empty
    };

    private static string BuildId(int eventId, int round, int number) => $"{eventId}-{round}-{number}";

    private static MatchStatus ToMatchStatus(int input) => input switch
    {
        0 => MatchStatus.Upcoming,
        1 => MatchStatus.Live,
        2 => MatchStatus.Ongoing,
        3 => MatchStatus.Finished,
        _ => MatchStatus.Unknown
    };

    private static MatchKind ToMatchKind(int input) => input switch
    {
        1 => MatchKind.Singles,
        2 => MatchKind.Team,
        3 => MatchKind.PartOfTeam,
        _ => MatchKind.Unknown
    };

    private static DateTime? ParseZuluUtc(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        if (!DateTimeOffset.TryParse(
                input,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var dto))
            return null;

        return dto.UtcDateTime;
    }

    private static string StripHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var s = Regex.Replace(
            input,
            @"<(script|style)[^>]*>.*?</\1>",
            "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        s = Regex.Replace(s, @"<[^>]+>", "");
        s = WebUtility.HtmlDecode(s);
        s = s.Replace('\u00A0', ' ');
        s = Regex.Replace(s, @"\s{2,}", " ");

        return s.Trim();
    }
}