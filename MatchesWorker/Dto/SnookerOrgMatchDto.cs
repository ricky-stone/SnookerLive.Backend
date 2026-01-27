namespace MatchesWorker.Dto;

public class SnookerOrgMatchDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int Round { get; set; }
    public int Number { get; set; }
    public int Player1Id { get; set; }
    public int Score1 { get; set; }
    public bool Walkover1 { get; set; }
    public int Player2Id { get; set; }
    public int Score2 { get; set; }
    public bool Walkover2 { get; set; }
    public int WinnerId { get; set; }
    public bool Unfinished { get; set; }
    public bool OnBreak { get; set; }
    public int Status { get; set; }
    public int WorldSnookerId { get; set; }
    public string LiveUrl { get; set; } = string.Empty;
    public string DetailsUrl { get; set; } = string.Empty;
    public bool PointsDropped { get; set; }
    public bool ShowCommonNote { get; set; }
    public bool Estimated { get; set; }
    public int Type { get; set; }
    public int TableNo { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public string InitDate { get; set; } = string.Empty;
    public string ModDate { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string ScheduledDate { get; set; } = string.Empty;
    public string FrameScores { get; set; } = string.Empty;
    public string Sessions { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string ExtendedNote { get; set; } = string.Empty;
    public bool HeldOver { get; set; }
    public string StatsUrl { get; set; } = string.Empty;
}