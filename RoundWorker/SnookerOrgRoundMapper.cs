using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Domain;

namespace SnookerLive;

public static class SnookerOrgRoundMapper
{
    public static RoundRecord ToRecord(SnookerOrgRoundInfoDto source)
    {
        return new RoundRecord
        {
            Id = $"{source.EventID}-{source.Round}",
            Round = source.Round,
            RoundName = source.RoundName,
            EventID = source.EventID,
            MainEvent = source.MainEvent,
            Distance = source.Distance,
            NumLeft = source.NumLeft,
            NumMatches = source.NumMatches,
            Note = source.Note,
            ValueType = source.ValueType,
            Rank = source.Rank,
            Money = source.Money,
            SeedGetsHalf = source.SeedGetsHalf,
            ActualMoney = source.ActualMoney,
            Currency = source.Currency,
            ConversionRate = source.ConversionRate,
            Points = source.Points,
            SeedPoints = source.SeedPoints
        };
    }
}