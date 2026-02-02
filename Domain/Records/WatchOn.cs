namespace Domain;

public sealed class WatchOn
{
    public string Id { get; set; } = string.Empty;
    public int SnookerOrgId { get; set; }
    public List<Link> Links { get; set; } = new();
}