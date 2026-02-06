namespace SnookerLive;

public class SnookerOrgPlayerDto
{
    public int Id { get; set; }
    public int Type { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int TeamNumber { get; set; }
    public int TeamSeason { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public string BioPage { get; set; } = string.Empty;
    public string Born { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public bool SurnameFirst { get; set; }
    public string License { get; set; } = string.Empty;
    public string Club { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public string PhotoSource { get; set; } = string.Empty;
    public int FirstSeasonAsPro { get; set; }
    public int LastSeasonAsPro { get; set; }
    public string Info { get; set; } = string.Empty;
    public int NumRankingTitles { get; set; }
    public int NumMaximums { get; set; }
    public string Died { get; set; } = string.Empty;
}