using Rabbit;

namespace MatchesWorker;

public class SessionMessage(string action) : BaseMessage(action)
{
    public DateTime? ScheduledStartDate { get; set; }
    public string Sessions { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
}