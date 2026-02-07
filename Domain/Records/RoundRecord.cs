namespace Domain;

public sealed class RoundRecord
{
    public string Id { get; set; } = string.Empty;
    public int Round { get; set; }
    public string RoundName { get; set; } = string.Empty;
    public int EventID { get; set; }    
    public int MainEvent { get; set; }
    public int Distance { get; set; }
    public int NumLeft { get; set; }
    public int NumMatches { get; set; }
    public string Note { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public int Rank { get; set; }
    public int Money { get; set; }
    public int SeedGetsHalf { get; set; }
    public int ActualMoney { get; set; }
    public string Currency { get; set; } = string.Empty;
    public double ConversionRate { get; set; }
    public int Points { get; set; }
    public int SeedPoints { get; set; }
    public int WinnerMoney { get; set; }
}