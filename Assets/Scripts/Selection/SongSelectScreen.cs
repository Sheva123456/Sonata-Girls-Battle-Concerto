using System;
using UnityEngine;

[Serializable]
public class SongEntry
{
    public string displayName;
    public SongChart chart;
}

public class SongSelectScreen : MonoBehaviour
{
    [SerializeField] SongEntry[] songs;
    [SerializeField] SongSelectSlot[] slots;

    void Start()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (songs != null && i < songs.Length)
                slots[i].Bind(this, i, songs[i]);
            else
                slots[i].BindEmpty();
        }

        if (songs != null && songs.Length > 0)
            Select(0);
    }

    public void Select(int index)
    {
        if (songs == null || index < 0 || index >= songs.Length || slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == index);
    }
}
