using Rabbit;

namespace SnookerLive;

public class SnookerOrgMessage(string action) : BaseMessage(action)
{
    public object?[] Url { get; set; } = [];
}