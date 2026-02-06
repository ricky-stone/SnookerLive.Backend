namespace SnookerLive;

public class SnookerOrgRoundInfoDto
{
    public int Round { get; init; }
    public string RoundName { get; init; } = string.Empty;
    public int EventID { get; init; }
    public int MainEvent { get; init; }
    public int Distance { get; init; }
    public int NumLeft { get; init; }
    public int NumMatches { get; init; }
    public string Note { get; init; } = string.Empty;
    public string ValueType { get; init; } = string.Empty;
    public int Rank { get; init; }
    public int Money { get; init; }
    public int SeedGetsHalf { get; init; }
    public int ActualMoney { get; init; }
    public string Currency { get; init; } = string.Empty;
    public double ConversionRate { get; init; }
    public int Points { get; init; }
    public int SeedPoints { get; init; }
}