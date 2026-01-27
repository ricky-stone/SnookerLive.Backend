using Rabbit;

namespace SnookerOrg.Messages;

public sealed class SnookerOrgMessage(string action): BaseMessage(action)
{
    public object?[] Url { get; set; } = [];
}