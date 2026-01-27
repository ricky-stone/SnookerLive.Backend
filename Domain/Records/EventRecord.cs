using Domain.Enums;

namespace Domain;

public class EventRecord
{
    public string Id { get; set; } = string.Empty;
    public int SnookerOrgId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Sponsor { get; set; } = string.Empty;
    public int Season { get; set; }
    public EventType Type { get; set; }
    public int Num { get; set; }
    public string Venue { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int Main { get; set; }
    public int QualifyingEventId { get; set; }
    public Gender Sex { get; set; }
    public Stage Stage { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public EventRankingType RankingType { get; set; }
    public bool Team { get; set; }
    public EventFormat Format { get; set; }
    public bool AllRoundsAdded { get; set; }
    public int NumCompetitors { get; set; }
    public int NumUpcoming { get; set; }
    public int NumActive { get; set; }
    public int NumResults { get; set; }
    public int PrizeMoney { get; set; }
    public int HighBreakMoney { get; set; }
    public string Note { get; set; } = string.Empty;
    public string CommonNote { get; set; } = string.Empty;
    public int DefendingChampionId { get; set; }
    public int PreviousEdition { get; set; }
    public Tour Tour { get; set; }
    public bool TripleCrown { get; set; }
    public bool HomeNations { get; set; }
}