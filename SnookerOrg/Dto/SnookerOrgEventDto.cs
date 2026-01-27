namespace SnookerOrg.Dto;

public class SnookerOrgEventDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Sponsor { get; set; } = string.Empty;
    public int Season { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Num { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Discipline { get; set; } = string.Empty;
    public int Main { get; set; }
    public string Sex { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Related { get; set; } = string.Empty;
    public string Stage { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public int WorldSnookerId { get; set; }
    public string RankingType { get; set; } = string.Empty;
    public int EventPredictionId { get; set; }
    public bool Team { get; set; }
    public int Format { get; set; }
    public string Twitter { get; set; } = string.Empty;
    public string HashTag { get; set; } = string.Empty;
    public double ConversionRate { get; set; }
    public bool AllRoundsAdded { get; set; }
    public string PhotoURLs { get; set; } = string.Empty;
    public int NumCompetitors { get; set; }
    public int NumUpcoming { get; set; }
    public int NumActive { get; set; }
    public int NumResults { get; set; }
    public string Note { get; set; } = string.Empty;
    public string CommonNote { get; set; } = string.Empty;
    public int DefendingChampion { get; set; }
    public int PreviousEdition { get; set; }
    public string Tour { get; set; } = string.Empty;
}