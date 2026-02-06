namespace SnookerLive;

public sealed class CandidateRecord
{
    public string Id { get; set; } = string.Empty;
    public int SnookerOrgId { get; set; }
    public int FromEvent { get; set; }
    public int FromRound { get; set; }
    public int FromNumber { get; set; }
    public int FromIndex { get; set; }
    public int ToEvent { get; set; }
    public int ToRound { get; set; }
    public int ToNumber { get; set; }
    public int ToIndex { get; set; }
    public int PlayerID { get; set; }
}