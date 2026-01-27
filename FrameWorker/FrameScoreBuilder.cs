using System.Text.RegularExpressions;
using Domain;

namespace FrameWorker;

public static class FrameScoreBuilder
{
    public static List<FrameRecord> BuildFrameScores(MatchRecord m)
    {
        var input = m.FrameScores;
        if (string.IsNullOrWhiteSpace(input))
            return new List<FrameRecord>();

        List<FrameRecord> scores = new List<FrameRecord>();
        input = Regex.Replace(input, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
        input = input.Replace("\r", "").Trim();

        var framePattern = @"(\d+)-(\d+)(?:\s*\(([^)]+)\))?";

        int frameNumber = 1;
        foreach (Match match in Regex.Matches(input, framePattern))
        {
            int p1 = int.Parse(match.Groups[1].Value);
            int p2 = int.Parse(match.Groups[2].Value);
            var frameScore = new FrameRecord
            {
                Id = $"{m.Id}-{frameNumber}",
                MatchId = m.Id,
                FrameNumber = frameNumber++,
                Player1FrameScore = p1,
                Player2FrameScore = p2
            };
            if (match.Groups[3].Success)
            {
                string breaksRaw = match.Groups[3].Value;
                var nums = new List<int>();

                foreach (Match b in Regex.Matches(breaksRaw, @"\d+"))
                    nums.Add(int.Parse(b.Value));

                if (nums.Count == 1)
                {
                    if (p1 > p2)
                        frameScore.Player1HighBreak = nums[0];
                    else
                        frameScore.Player2HighBreak = nums[0];
                }
                else if (nums.Count == 2)
                {
                    frameScore.Player1HighBreak = nums[0];
                    frameScore.Player2HighBreak = nums[1];
                }
            }

            scores.Add(frameScore);
        }

        return scores;
    }
}