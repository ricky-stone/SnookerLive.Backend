using Rabbit;

namespace SnookerOrg.Messages;

public sealed class SnookerOrgMessage(string action): BaseMessage(action)
{
    public string Guid { get; set; } = string.Empty;
    public object?[] Url { get; set; } = [];
    public string PublishTo { get; set; } = string.Empty;
}