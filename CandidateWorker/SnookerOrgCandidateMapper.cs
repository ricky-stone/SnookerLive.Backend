namespace SnookerLive;

public static class SnookerOrgCandidateMapper
{
    public static CandidateRecord ToCandidate(this SnookerOrgCandidateDto dto)
    {
        return new CandidateRecord
        {
            Id = $"{dto.ToEvent}-{dto.ToRound}-{dto.ToNumber}-{dto.ToIndex}",
            SnookerOrgId = dto.Id,
            FromEvent = dto.FromEvent,
            FromRound = dto.FromRound,
            FromNumber = dto.FromNumber,
            FromIndex = dto.FromIndex,
            ToEvent = dto.ToEvent,
            ToRound = dto.ToRound,
            ToNumber = dto.ToNumber,
            ToIndex = dto.ToIndex,
            PlayerID = dto.PlayerID
        };
    }
}