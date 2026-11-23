using System;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] SongChart chart;
    [SerializeField] SongClock clock;
    [SerializeField] RhythmInput rhythmInput;
    [SerializeField] NoteView notePrefab;
    [SerializeField] LaneTrack[] tracks;
    [SerializeField] Text judgeText;
    [SerializeField] Text timeText;
    [SerializeField] float approachSeconds = 1.2f;
    [SerializeField] float hitWindowSeconds = 0.1f;

    readonly List<RuntimeNote> notes = new List<RuntimeNote>();
    int hits;
    int misses;
    bool started;
    bool laidOut;

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
        if (!started || clock == null || !clock.IsPlaying)
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
            if (!note.resolved && songTime > note.hitTime + hitWindowSeconds)
            {
                note.resolved = true;
                misses++;
                if (note.view != null)
                    note.view.MarkMiss();
                SetJudge("Miss", new Color(1f, 0.4f, 0.34f));
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

        if (timeText == null)
            return;

        if (notes.Count == 0)
            timeText.text = $"Done    {hits} hit    {misses} miss";
        else
            timeText.text = $"{Mathf.Max(0f, (float)songTime):0.00}s    {hits} hit    {misses} miss";
    }

    void TryHit(RhythmLane lane, double songTime)
    {
        RuntimeNote best = null;
        float bestDistance = hitWindowSeconds;
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

        best.resolved = true;
        hits++;
        if (best.view != null)
            Destroy(best.view.gameObject);
        best.view = null;
        Flash(FindTrack(lane));
        SetJudge("Hit", new Color(0.55f, 1f, 0.62f));
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
