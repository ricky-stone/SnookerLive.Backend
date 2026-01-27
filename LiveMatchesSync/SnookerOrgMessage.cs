using Rabbit;

namespace LiveMatchesSync;

public class SnookerOrgMessage(string action) : BaseMessage(action)
{
    public object?[] Url { get; set; } = [];
}