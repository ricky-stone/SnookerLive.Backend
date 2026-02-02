using Rabbit;

namespace SnookerLive;

public sealed class WatchOnMessage(string action, string id, int SnookerOrgId,  string watchOnString) : BaseMessage(action)
{
    public string Id { get; set; } = id;
    public int SnookerOrgId { get; set; } = SnookerOrgId;
    public string WatchOnString { get; } = watchOnString;
}