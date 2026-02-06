namespace Domain;

public class RankingRecord
{
    public string Id { get; set; } = string.Empty;
    public int Position { get; set; }
    public int PlayerId { get; set; }
    public int Season { get; set; }
    public int Sum  { get; set; }
    public string Type { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
}