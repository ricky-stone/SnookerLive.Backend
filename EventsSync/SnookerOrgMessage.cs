using Rabbit;

namespace EventsSync;

public class SnookerOrgMessage(string action) : BaseMessage(action)
{
    public object?[] Url { get; set; } = [];
}