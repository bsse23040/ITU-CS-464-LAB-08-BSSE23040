using UnityEngine;

// The one player for every interface sound. It has a single AudioSource on the UI mixer group, so no button
// needs its own. Buttons ask it through UiSound; screen changes and success or error reach it as events.
public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance { get; private set; }

    [SerializeField] AudioSource source;          // Output: the UI group. Keeps playing while the game is paused
    [SerializeField] ScreenManager screens;

    [Header("Clips")]
    [SerializeField] AudioClip hover;
    [SerializeField] AudioClip click;
    [SerializeField] AudioClip back;
    [SerializeField] AudioClip open;
    [SerializeField] AudioClip close;
    [SerializeField] AudioClip pause;
    [SerializeField] AudioClip resume;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip error;

    float lastHover;

    void Awake()
    {
        Instance = this;
        source.ignoreListenerPause = true;        // the pause menu has to click too
    }

    void OnEnable()
    {
        screens.ScreenChanged += OnScreenChanged;
        UiFeedback.Succeeded += PlaySuccess;
        UiFeedback.Failed += PlayError;
    }

    void OnDisable()
    {
        screens.ScreenChanged -= OnScreenChanged;
        UiFeedback.Succeeded -= PlaySuccess;
        UiFeedback.Failed -= PlayError;
    }

    public void Hover()
    {
        if (Time.unscaledTime - lastHover < 0.06f) return;     // a finger sliding over a grid should not rattle
        lastHover = Time.unscaledTime;
        Play(hover, 0.05f, 0.7f);
    }

    public void Click() { Play(click, 0.05f, 1f); }
    public void PlayError() { Play(error, 0f, 1f); }
    public void PlaySuccess() { Play(success, 0f, 1f); }

    // opening and closing panels. Pause and resume have their own pair, and so does going back
    void OnScreenChanged(ScreenId from, ScreenId to, NavKind kind)
    {
        if (from == to) return;                                     // the first screen at launch
        if (to == ScreenId.Pause && from == ScreenId.Gameplay) Play(pause, 0f, 1f);
        else if (from == ScreenId.Pause && to == ScreenId.Gameplay) Play(resume, 0f, 1f);
        else if (to == ScreenId.Gameplay || to == ScreenId.Results) return;   // the game and the results have their own sounds
        else if (kind == NavKind.Back) Play(back, 0f, 1f);
        else if (kind == NavKind.Fresh) Play(close, 0f, 1f);        // closing what was open and starting over
        else Play(open, 0f, 1f);
    }

    void Play(AudioClip clip, float pitchJitter, float volume)
    {
        if (clip == null) return;
        source.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        source.PlayOneShot(clip, volume);
    }
}
