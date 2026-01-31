using Rabbit;

namespace UpcomingEventMatchesSync;

public class SnookerOrgMessage(string action) : BaseMessage(action)
{
    public object?[] Url { get; set; } = [];
}