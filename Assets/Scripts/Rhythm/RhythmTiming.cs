public enum RhythmJudgment
{
    Perfect,
    Great,
    Good,
    Bad,
    Miss
}

// Windows are the absolute distance from the note's hit time.
// Perfect, Great, and Good land the move. Bad still takes the note, but the move does not land.
// Anything later than Bad is a Miss and can no longer be hit.
public static class RhythmTiming
{
    public const float PerfectSeconds = 0.040f;
    public const float GreatSeconds = 0.080f;
    public const float GoodSeconds = 0.120f;
    public const float BadSeconds = 0.160f;

    public static RhythmJudgment Judge(float absoluteSeconds)
    {
        if (absoluteSeconds <= PerfectSeconds)
            return RhythmJudgment.Perfect;
        if (absoluteSeconds <= GreatSeconds)
            return RhythmJudgment.Great;
        if (absoluteSeconds <= GoodSeconds)
            return RhythmJudgment.Good;
        if (absoluteSeconds <= BadSeconds)
            return RhythmJudgment.Bad;
        return RhythmJudgment.Miss;
    }

    public static bool IsSuccess(RhythmJudgment judgment)
    {
        return judgment == RhythmJudgment.Perfect
            || judgment == RhythmJudgment.Great
            || judgment == RhythmJudgment.Good;
    }
}
