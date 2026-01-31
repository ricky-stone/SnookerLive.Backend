using Domain;
using Rabbit;

namespace SnookerLive;

public class FrameMessage(string action, MatchRecord match) : BaseMessage(action)
{
    public MatchRecord Match { get; set; } = match;
}