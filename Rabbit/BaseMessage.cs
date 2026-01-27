namespace Rabbit;

public abstract class BaseMessage(string action)
{
    public string Action { get; } = action;
}