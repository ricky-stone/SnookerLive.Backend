using Rabbit;

namespace SnookerLive;

public sealed class SnookerOrgDataResponse(string action) : BaseMessage(action)
{
    public string Data { get; set; } = string.Empty;
}