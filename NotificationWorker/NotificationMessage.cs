using Rabbit;

namespace NotificationWorker;

public sealed class NotificationMessage(string action) : BaseMessage(action)
{
    public required string Data { get; set; }
}