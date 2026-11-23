using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectSlot : MonoBehaviour, IPointerClickHandler
{
    static readonly Color SelectedColor = new Color(0.96f, 0.78f, 0.22f, 1f);
    static readonly Color NormalColor = new Color(0.10f, 0.24f, 0.42f, 1f);

    [SerializeField] Image portrait;
    [SerializeField] Image frame;

    CharacterSelectScreen screen;
    int index;

    public void Bind(CharacterSelectScreen owner, int slotIndex, Sprite idle)
    {
        screen = owner;
        index = slotIndex;
        if (portrait == null)
            return;

        portrait.sprite = idle;
        portrait.enabled = idle != null;
    }

    public void SetSelected(bool selected)
    {
        if (frame != null)
            frame.color = selected ? SelectedColor : NormalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (screen != null)
            screen.Select(index);
    }
}
