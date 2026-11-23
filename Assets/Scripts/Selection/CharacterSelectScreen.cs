using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RosterEntry
{
    public string displayName;
    public Sprite idle;
}

public class CharacterSelectScreen : MonoBehaviour
{
    [SerializeField] RosterEntry[] roster;
    [SerializeField] CharacterSelectSlot[] slots;
    [SerializeField] Image preview;
    [SerializeField] Text nameLabel;

    void Start()
    {
        if (slots == null || roster == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            Sprite idle = i < roster.Length ? roster[i].idle : null;
            slots[i].Bind(this, i, idle);
        }

        Select(0);
    }

    public void Select(int index)
    {
        if (roster == null || index < 0 || index >= roster.Length)
            return;

        if (preview != null)
            preview.sprite = roster[index].idle;
        if (nameLabel != null)
            nameLabel.text = roster[index].displayName;

        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == index);
    }
}
