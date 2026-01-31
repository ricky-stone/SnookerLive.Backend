namespace SnookerLive;

public abstract class IController
{
    public string Action { get; init; } = string.Empty;
    public abstract Task HandleMessage(string message);
    public abstract bool Validate(string message);
}