using System.Net;
using Domain;

namespace SnookerLive;

public static class SnookerOrgRankingMapper
{
    public static RankingRecord ToRankingRecord(string valueType, SnookerOrgRankingDto ranking)
    {

        return new RankingRecord
        {
            Id = $"{ranking.PlayerId}-{ranking.Season}-{valueType}",
            Position = ranking.Position,
            PlayerId = ranking.PlayerId,
            Season = ranking.Season,
            Sum = ranking.Sum,
            Type = ranking.Type,
            ValueType = valueType
        };
    }
}