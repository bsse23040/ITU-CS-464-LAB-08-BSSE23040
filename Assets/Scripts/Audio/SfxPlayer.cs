using UnityEngine;

// Gameplay sound effects. A few AudioSources on the SFX group take turns and use PlayOneShot, so short sounds
// can overlap. Repeated sounds get a slightly different pitch each time so they do not sound mechanical.
// These sources stop with AudioListener.pause, so a sound that was playing waits for the player to resume.
// The preview source ignores the pause, so the SFX slider can be heard while the settings are open from the pause menu.
public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] AudioSource[] voices;
    [SerializeField] AudioSource preview;
    [SerializeField] AudioClip previewClip;

    int next;
    float lastPreview;

    void Awake()
    {
        Instance = this;
        preview.ignoreListenerPause = true;
    }

    public void Play(AudioClip clip, float pitchJitter = 0f, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource s = voices[next];
        next = (next + 1) % voices.Length;
        s.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        s.PlayOneShot(clip, volume);
    }

    public void Preview()
    {
        if (Time.unscaledTime - lastPreview < 0.18f) return;
        lastPreview = Time.unscaledTime;
        preview.PlayOneShot(previewClip);
    }

    // a new run must not start with the end of the last one still ringing
    public void StopAll()
    {
        foreach (var s in voices) s.Stop();
    }
}
