using UnityEngine;
using UnityEngine.UI;

public class NoteView : MonoBehaviour
{
    static readonly Color AttackColor = new Color(1f, 0.62f, 0.28f, 1f);
    static readonly Color DefenseColor = new Color(0.45f, 0.82f, 1f, 1f);
    static readonly Color MissColor = new Color(0.35f, 0.35f, 0.35f, 0.75f);

    [SerializeField] Image image;
    [SerializeField] Text label;

    public RhythmLane Lane { get; private set; }
    public float HitTime { get; private set; }

    public void Bind(RhythmLane lane, float hitTime)
    {
        Lane = lane;
        HitTime = hitTime;

        if (label != null)
        {
            label.text = RhythmLaneInfo.Glyph(lane);
            if (label.font == null)
                label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        if (image != null)
            image.color = RhythmLaneInfo.IsAttack(lane) ? AttackColor : DefenseColor;

        SetVisible(false);
    }

    public float TravelT(double songTime, float approachSeconds)
    {
        if (approachSeconds <= 0f)
            return 1f;
        return 1f - (HitTime - (float)songTime) / approachSeconds;
    }

    public void Place(double songTime, float approachSeconds, float receptorX)
    {
        float t = TravelT(songTime, approachSeconds);
        var parent = (RectTransform)transform.parent;
        float spawnX = Mathf.Max(receptorX + 120f, parent.rect.width - 36f);
        var rect = (RectTransform)transform;
        rect.anchoredPosition = new Vector2(Mathf.Lerp(spawnX, receptorX, t), 0f);
        SetVisible(t >= 0f && t <= 1.25f);
    }

    public void MarkMiss()
    {
        if (image != null)
            image.color = MissColor;
    }

    void SetVisible(bool visible)
    {
        if (image != null)
            image.enabled = visible;
        if (label != null)
            label.enabled = visible;
    }
}
