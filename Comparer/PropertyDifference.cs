namespace Comparer;

public class PropertyDifference
{
    public string PropertyName { get; set; } = string.Empty;
    public object? Value1 { get; set; }
    public object? Value2 { get; set; }
    public override string ToString()
    {
        return $"{PropertyName}: '{Value1 ?? "null"}' vs '{Value2 ?? "null"}'";
    }
}