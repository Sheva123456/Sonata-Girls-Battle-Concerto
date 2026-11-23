using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SongSelectSlot : MonoBehaviour, IPointerClickHandler
{
    static readonly Color SelectedColor = new Color(0.62f, 0.22f, 0.20f, 1f);
    static readonly Color NormalColor = new Color(0.12f, 0.09f, 0.11f, 1f);

    [SerializeField] Image frame;
    [SerializeField] Text title;
    [SerializeField] Text detail;

    SongSelectScreen screen;
    int index;
    bool filled;

    public void Bind(SongSelectScreen owner, int slotIndex, SongEntry entry)
    {
        screen = owner;
        index = slotIndex;
        filled = entry != null && entry.chart != null;

        if (title != null)
            title.text = entry != null ? entry.displayName : string.Empty;

        if (detail != null)
        {
            detail.text = filled
                ? Mathf.RoundToInt(entry.chart.bpm) + " BPM"
                : string.Empty;
        }
    }

    public void BindEmpty()
    {
        filled = false;
        screen = null;
        if (title != null)
            title.text = string.Empty;
        if (detail != null)
            detail.text = string.Empty;
    }

    public void SetSelected(bool selected)
    {
        if (!filled || frame == null)
            return;

        frame.color = selected ? SelectedColor : NormalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (filled && screen != null)
            screen.Select(index);
    }
}
