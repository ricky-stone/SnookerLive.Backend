namespace Domain;

public class PlayerRecord
{
    public string Id { get; set; } = string.Empty;
    public int SnookerOrgId { get; set; }
    public int Type { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayFirstName { get; set; } = string.Empty;
    public string DisplayLastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DisplayFullName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int TeamNumber { get; set; }
    public int TeamSeason { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public string Born { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public bool SurnameFirst { get; set; }
    public int FirstSeasonAsPro { get; set; }
    public int LastSeasonAsPro { get; set; }
    public string Info { get; set; } = string.Empty;
    public int NumRankingTitles { get; set; }
    public int NumMaximums { get; set; }
    public string Died { get; set; } = string.Empty;
}