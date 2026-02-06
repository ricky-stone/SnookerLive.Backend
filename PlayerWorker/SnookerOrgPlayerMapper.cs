using Domain;

namespace SnookerLive;

public static class SnookerOrgPlayerMapper
{
    public static PlayerRecord ToPlayerRecord(SnookerOrgPlayerDto player) => new()
    {
        Id = $"{player.Id}",
        SnookerOrgId = player.Id,
        FirstName = player.FirstName,
        MiddleName = player.MiddleName,
        LastName = player.LastName,
        FullName = BuildFullName(player),
        DisplayFirstName = BuildDisplayFirstName(player),
        DisplayLastName = BuildDisplayLastName(player),
        DisplayFullName = BuildDisplayFullName(player),
        TeamName = player.TeamName,
        TeamNumber = player.TeamNumber,
        TeamSeason = player.TeamSeason,
        ShortName = player.ShortName,
        Nationality = player.Nationality,
        Sex = player.Sex,
        Born = player.Born,
        Twitter = player.Twitter,
        SurnameFirst = player.SurnameFirst,
        FirstSeasonAsPro = player.FirstSeasonAsPro,
        LastSeasonAsPro = player.LastSeasonAsPro,
        Info = player.Info,
        NumRankingTitles = player.NumRankingTitles,
        NumMaximums = player.NumMaximums,
        Died = player.Died
    };

    private static (string First, string Last) GetDisplayNameParts(SnookerOrgPlayerDto p) =>
    p.SurnameFirst
        ? (p.LastName, p.FirstName)
        : (p.FirstName, p.LastName);

    private static string BuildDisplayFirstName(SnookerOrgPlayerDto p) =>
        GetDisplayNameParts(p).First;

    private static string BuildDisplayLastName(SnookerOrgPlayerDto p) =>
        GetDisplayNameParts(p).Last;

    private static string BuildFullName(SnookerOrgPlayerDto p) =>
        $"{p.FirstName} {p.LastName}";

    private static string BuildDisplayFullName(SnookerOrgPlayerDto p)
    {
        var (first, last) = GetDisplayNameParts(p);
        return $"{first} {last}";
    }
}