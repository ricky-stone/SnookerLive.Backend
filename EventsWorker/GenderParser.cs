using Domain.Enums;

namespace EventsWorker;

public class GenderParser
{
    public static Gender NormalizeGender(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Gender.Unknown;

        return input.Trim().ToLowerInvariant() switch
        {
            "m" or "male" => Gender.Male,
            "f" or "female" => Gender.Female,
            "both" => Gender.Both,
            _ => Gender.Unknown
        };
    }
}