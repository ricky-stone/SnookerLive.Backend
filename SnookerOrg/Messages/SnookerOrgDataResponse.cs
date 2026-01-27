using Rabbit;

namespace SnookerOrg.Messages;

public sealed class SnookerOrgDataResponse(string action) : BaseMessage(action)
{
    public string Data { get; set; } = string.Empty;
}