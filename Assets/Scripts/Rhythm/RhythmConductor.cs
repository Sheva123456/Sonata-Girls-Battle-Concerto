using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class LaneTrack
{
    public bool attack;
    public RectTransform noteParent;
    public Image receptorImage;
    public float receptorX = 108f;

    [NonSerialized] public Color restingColor;
    [NonSerialized] public float flashUntil;
}

public class RhythmConductor : MonoBehaviour
{
    const string ResultsScene = "BattleResultsScene";
    const float ResultsDelaySeconds = 0.75f;

    [SerializeField] SongChart chart;
    [SerializeField] SongClock clock;
    [SerializeField] RhythmInput rhythmInput;
    [SerializeField] NoteView notePrefab;
    [SerializeField] LaneTrack[] tracks;
    [SerializeField] Text judgeText;
    [SerializeField] Text timeText;
    [SerializeField] CharacterPose leftCharacter;
    [SerializeField] float approachSeconds = 1.2f;

    readonly List<RuntimeNote> notes = new List<RuntimeNote>();
    int perfect;
    int great;
    int good;
    int bad;
    int miss;
    int successfulAttacks;
    int successfulDefends;
    bool started;
    bool laidOut;
    bool ending;
    bool reported;
    float showResultsAt;

    class RuntimeNote
    {
        public RhythmLane lane;
        public float hitTime;
        public NoteView view;
        public bool resolved;
    }

    void Start()
    {
        if (chart == null || clock == null || notePrefab == null)
        {
            Debug.LogError("RhythmConductor needs a chart, a clock, and a note prefab.");
            return;
        }

        BattleResults.Current.Clear();

        foreach (var track in tracks)
        {
            if (track.receptorImage != null)
                track.restingColor = track.receptorImage.color;
        }

        SetJudge("Hit the circle", Color.white);
        clock.Play(chart.audioClip, chart.offsetMs / 1000d);
        SpawnAll();
        started = true;
    }

    void SpawnAll()
    {
        foreach (var note in chart.notes)
        {
            var track = FindTrack(note.lane);
            if (track == null || track.noteParent == null)
            {
                Debug.LogError($"No note parent for {note.lane}.");
                continue;
            }

            var view = Instantiate(notePrefab, track.noteParent);
            float hitTime = (float)chart.BeatToSeconds(note.beat);
            view.Bind(note.lane, hitTime);
            notes.Add(new RuntimeNote
            {
                lane = note.lane,
                hitTime = hitTime,
                view = view
            });
        }
    }

    void Update()
    {
        if (!started || clock == null || !clock.IsPlaying || reported)
            return;

        if (!laidOut)
        {
            Canvas.ForceUpdateCanvases();
            laidOut = true;
        }

        double songTime = clock.SongTime;
        RestoreFlashes();

        if (rhythmInput != null)
        {
            foreach (var lane in rhythmInput.PressedThisFrame)
                TryHit(lane, songTime);
        }

        for (int i = notes.Count - 1; i >= 0; i--)
        {
            var note = notes[i];
            if (!note.resolved && songTime > note.hitTime + RhythmTiming.BadSeconds)
            {
                note.resolved = true;
                Record(RhythmJudgment.Miss, note.lane);
                if (note.view != null)
                    note.view.MarkMiss();
                SetJudge("MISS", new Color(1f, 0.4f, 0.34f));
                if (leftCharacter != null)
                    leftCharacter.ShowIdle();
            }

            if (note.view == null)
            {
                notes.RemoveAt(i);
                continue;
            }

            var track = FindTrack(note.lane);
            float receptorX = track != null ? track.receptorX : 108f;
            note.view.Place(songTime, approachSeconds, receptorX);

            if (note.resolved && note.view.TravelT(songTime, approachSeconds) > 1.15f)
            {
                Destroy(note.view.gameObject);
                notes.RemoveAt(i);
            }
        }

        WriteTally(songTime);

        if (!ending && SongFinished(songTime))
        {
            ending = true;
            showResultsAt = Time.unscaledTime + ResultsDelaySeconds;
        }

        if (ending && Time.unscaledTime >= showResultsAt)
            ShowResults();
    }

    bool SongFinished(double songTime)
    {
        for (int i = 0; i < notes.Count; i++)
        {
            if (!notes[i].resolved)
                return false;
        }

        if (chart.audioClip != null && songTime < chart.audioClip.length)
            return false;

        return true;
    }

    void TryHit(RhythmLane lane, double songTime)
    {
        RuntimeNote best = null;
        float bestDistance = RhythmTiming.BadSeconds;
        foreach (var note in notes)
        {
            if (note.resolved || note.lane != lane)
                continue;

            float distance = Mathf.Abs(note.hitTime - (float)songTime);
            if (distance <= bestDistance)
            {
                bestDistance = distance;
                best = note;
            }
        }

        if (best == null)
            return;

        RhythmJudgment judgment = RhythmTiming.Judge(bestDistance);
        best.resolved = true;
        Record(judgment, lane);
        if (best.view != null)
            Destroy(best.view.gameObject);
        best.view = null;
        Flash(FindTrack(lane));
        SetJudge(JudgmentName(judgment), JudgmentColor(judgment));

        if (leftCharacter == null)
            return;

        if (!RhythmTiming.IsSuccess(judgment))
        {
            leftCharacter.ShowIdle();
            return;
        }

        if (RhythmLaneInfo.IsAttack(lane))
            leftCharacter.ShowAttack();
        else
            leftCharacter.ShowDefense();
    }

    void Record(RhythmJudgment judgment, RhythmLane lane)
    {
        switch (judgment)
        {
            case RhythmJudgment.Perfect:
                perfect++;
                break;
            case RhythmJudgment.Great:
                great++;
                break;
            case RhythmJudgment.Good:
                good++;
                break;
            case RhythmJudgment.Bad:
                bad++;
                break;
            default:
                miss++;
                break;
        }

        if (!RhythmTiming.IsSuccess(judgment))
            return;

        if (RhythmLaneInfo.IsAttack(lane))
            successfulAttacks++;
        else
            successfulDefends++;
    }

    void ShowResults()
    {
        reported = true;
        BattleResults results = BattleResults.Current;
        results.Clear();
        results.winnerName = "PLAYER 1";
        results.perfect = perfect;
        results.great = great;
        results.good = good;
        results.bad = bad;
        results.miss = miss;
        results.successfulAttacks = successfulAttacks;
        results.successfulDefends = successfulDefends;
        SceneManager.LoadScene(ResultsScene);
    }

    void WriteTally(double songTime)
    {
        if (timeText == null)
            return;

        string clock = Mathf.Max(0f, (float)songTime).ToString("0.00");
        timeText.text = $"{clock}s    Perfect {perfect}    Great {great}    Good {good}    Bad {bad}    Miss {miss}";
    }

    static string JudgmentName(RhythmJudgment judgment)
    {
        switch (judgment)
        {
            case RhythmJudgment.Perfect: return "PERFECT";
            case RhythmJudgment.Great: return "GREAT";
            case RhythmJudgment.Good: return "GOOD";
            case RhythmJudgment.Bad: return "BAD";
            default: return "MISS";
        }
    }

    static Color JudgmentColor(RhythmJudgment judgment)
    {
        switch (judgment)
        {
            case RhythmJudgment.Perfect: return new Color(1f, 0.84f, 0.28f);
            case RhythmJudgment.Great: return new Color(0.45f, 0.95f, 0.55f);
            case RhythmJudgment.Good: return new Color(0.45f, 0.82f, 1f);
            case RhythmJudgment.Bad: return new Color(1f, 0.58f, 0.28f);
            default: return new Color(1f, 0.4f, 0.34f);
        }
    }

    void Flash(LaneTrack track)
    {
        if (track == null || track.receptorImage == null)
            return;

        track.receptorImage.color = Color.white;
        track.flashUntil = Time.unscaledTime + 0.08f;
    }

    void RestoreFlashes()
    {
        if (tracks == null)
            return;

        foreach (var track in tracks)
        {
            if (track.receptorImage == null || track.flashUntil <= 0f)
                continue;
            if (Time.unscaledTime < track.flashUntil)
                continue;

            track.receptorImage.color = track.restingColor;
            track.flashUntil = 0f;
        }
    }

    LaneTrack FindTrack(RhythmLane lane)
    {
        if (tracks == null)
            return null;

        bool attack = RhythmLaneInfo.IsAttack(lane);
        foreach (var track in tracks)
        {
            if (track.attack == attack)
                return track;
        }

        return null;
    }

    void SetJudge(string message, Color color)
    {
        if (judgeText == null)
            return;

        judgeText.text = message;
        judgeText.color = color;
    }
}
