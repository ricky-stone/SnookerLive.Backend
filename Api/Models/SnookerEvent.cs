using Domain;

namespace SnookerLive;

public sealed class SnookerEvent
{
    public string Id { get; set; } = string.Empty;
    public required EventRecord Event { get; set; }
    public List<WatchOn>? WatchOn { get; set; }
    public List<RoundRecord>? Rounds { get; set; }
}