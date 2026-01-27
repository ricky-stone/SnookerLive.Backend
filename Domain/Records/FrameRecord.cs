namespace Domain;

public class FrameRecord
{
    public string Id { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public int Player1HighBreak { get; set; }
    public int Player1FrameScore { get; set; }
    public int FrameNumber { get; set; }
    public int Player2FrameScore { get; set; }
    public int Player2HighBreak { get; set; }
}