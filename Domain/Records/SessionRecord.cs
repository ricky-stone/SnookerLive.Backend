namespace Domain;

public class SessionRecord
{
    public string Id { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public int SessionNumber { get; set; }
    public DateTime Utc { get; set; }
    public bool IsExplicitTime { get; set; }
}