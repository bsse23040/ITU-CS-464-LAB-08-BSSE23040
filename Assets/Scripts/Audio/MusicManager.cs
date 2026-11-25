using UnityEngine;

// The background music. Two AudioSources on the Music group take turns: to change track, the new one fades in
// on the free source while the old one fades out (a crossfade).
//   menus      calm menu track
//   gameplay   faster game track
//   results    warm results track
// While paused the music keeps playing but ducks down, so the pause menu does not go dead silent.
public class MusicManager : MonoBehaviour
{
    [SerializeField] ScreenManager screens;
    [SerializeField] AudioSource sourceA;
    [SerializeField] AudioSource sourceB;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] AudioClip resultsMusic;
    [SerializeField] float crossfadeSeconds = 1.5f;
    [SerializeField] float pausedLevel = 0.35f;     // how loud the music is while paused, as a part of normal

    AudioSource current, previous;
    float fade = 1f;                               // 0 just switched, 1 the crossfade is done
    float duck = 1f, duckTarget = 1f;

    public AudioClip CurrentClip => current != null ? current.clip : null;
    public AudioClip GameClip => gameMusic;

    void Awake()
    {
        foreach (var s in new[] { sourceA, sourceB })
        {
            s.loop = true;
            s.volume = 0f;
            s.ignoreListenerPause = true;           // music carries on when the pause menu stops the listener
        }
    }

    void OnEnable() { screens.ScreenChanged += OnScreenChanged; }
    void OnDisable() { screens.ScreenChanged -= OnScreenChanged; }

    void OnScreenChanged(ScreenId from, ScreenId to, NavKind kind)
    {
        bool inRun = screens.InStack(ScreenId.Gameplay);
        duckTarget = to == ScreenId.Pause || (to == ScreenId.Settings && inRun) ? pausedLevel : 1f;

        if (to == ScreenId.Loading) return;         // keep whatever is playing while the level loads
        if (to == ScreenId.Results) Play(resultsMusic);
        else if (to == ScreenId.Gameplay || inRun) Play(gameMusic);
        else Play(menuMusic);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null || (current != null && current.clip == clip)) return;
        previous = current;
        current = current == sourceA ? sourceB : sourceA;
        current.clip = clip;
        current.volume = 0f;
        current.Play();
        fade = previous == null ? 0.7f : 0f;        // the very first track comes in quickly
    }

    void Update()
    {
        if (current == null) return;
        fade = Mathf.MoveTowards(fade, 1f, Time.unscaledDeltaTime / crossfadeSeconds);
        duck = Mathf.MoveTowards(duck, duckTarget, Time.unscaledDeltaTime * 2.5f);

        // equal power: the two volumes follow a quarter circle, so the middle of the fade does not dip
        current.volume = Mathf.Sin(fade * Mathf.PI * 0.5f) * duck;
        if (previous != null)
        {
            previous.volume = Mathf.Cos(fade * Mathf.PI * 0.5f) * duck;
            if (fade >= 1f) { previous.Stop(); previous = null; }
        }
    }
}
