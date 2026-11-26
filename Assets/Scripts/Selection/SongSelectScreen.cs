using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] Image leftPortrait;
    [SerializeField] Text leftName;

    int selectedIndex;

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

        ApplyPickedCharacter();
    }

    void ApplyPickedCharacter()
    {
        MatchSetup match = MatchSetup.Current;
        if (!match.hasCharacter)
            return;

        if (leftPortrait != null && match.leftIdle != null)
            leftPortrait.sprite = match.leftIdle;

        if (leftName != null)
        {
            leftName.text = string.IsNullOrEmpty(match.displayName)
                ? "PLACEHOLDER 1"
                : match.displayName.ToUpperInvariant();
        }
    }

    public void Select(int index)
    {
        if (songs == null || index < 0 || index >= songs.Length || slots == null)
            return;

        selectedIndex = index;

        for (int i = 0; i < slots.Length; i++)
            slots[i].SetSelected(i == index);
    }

    public void StartBattle()
    {
        if (songs == null || selectedIndex < 0 || selectedIndex >= songs.Length)
            return;

        SongEntry entry = songs[selectedIndex];
        if (entry == null || entry.chart == null)
            return;

        MatchSetup.Current.SetSong(entry.displayName, entry.chart);
        SceneManager.LoadScene("RhythmBattleScene");
    }
}
