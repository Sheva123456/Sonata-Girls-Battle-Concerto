using UnityEngine;

// Song time comes from the audio DSP clock, so a hitch cannot drift the notes off the clip.
public class SongClock : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] double startDelay = 1d;

    double startDspTime;
    double offsetSeconds;

    public double SongTime { get; private set; }
    public bool IsPlaying { get; private set; }

    public void Play(AudioClip clip, double offsetSeconds)
    {
        this.offsetSeconds = offsetSeconds;
        startDspTime = AudioSettings.dspTime + startDelay;
        SongTime = -startDelay;
        IsPlaying = true;

        if (audioSource == null || clip == null)
            return;

        audioSource.playOnAwake = false;
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.PlayScheduled(startDspTime);
    }

    void Update()
    {
        if (!IsPlaying)
            return;

        SongTime = AudioSettings.dspTime - startDspTime - offsetSeconds;
    }
}
