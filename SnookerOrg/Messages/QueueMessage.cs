using SnookerOrg.Enums;

namespace SnookerOrg.Messages;

public record QueueMessage(Priority Priority, SnookerOrgMessage Message);