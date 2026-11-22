using System;
using System.Collections.Generic;
using UnityEngine;

public enum RhythmLane
{
    AttackUp = 0,
    AttackLeft = 1,
    AttackDown = 2,
    AttackRight = 3,
    DefenseUp = 4,
    DefenseLeft = 5,
    DefenseDown = 6,
    DefenseRight = 7
}

public static class RhythmLaneInfo
{
    public static bool IsAttack(RhythmLane lane)
    {
        return lane <= RhythmLane.AttackRight;
    }

    public static string Glyph(RhythmLane lane)
    {
        switch (lane)
        {
            case RhythmLane.AttackUp: return "W";
            case RhythmLane.AttackLeft: return "A";
            case RhythmLane.AttackDown: return "S";
            case RhythmLane.AttackRight: return "D";
            case RhythmLane.DefenseUp: return "^";
            case RhythmLane.DefenseLeft: return "<";
            case RhythmLane.DefenseDown: return "v";
            case RhythmLane.DefenseRight: return ">";
            default: return "?";
        }
    }
}

[Serializable]
public class ChartNote
{
    public float beat;
    public RhythmLane lane;
}

[CreateAssetMenu(fileName = "SongChart", menuName = "Rhythm/Song Chart")]
public class SongChart : ScriptableObject
{
    public string songId = "test-loop";

    public float bpm = 120f;

    [Tooltip("Positive values make the chart reach the circles later. The clock applies this once. Beats stay on the grid.")]
    public int offsetMs;

    public AudioClip audioClip;

    public List<ChartNote> notes = new List<ChartNote>();

    public double BeatToSeconds(float beat)
    {
        if (bpm <= 0f)
            return 0d;
        return beat * (60d / bpm);
    }
}
