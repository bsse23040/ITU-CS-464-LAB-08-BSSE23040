using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Loads the Gameplay scene before every run, and shows how far it has really got:
//   0 to 70 %    the scene loading in the background (AsyncOperation.progress, which stops at 0.9)
//   70 to 100 %  the audio data of the gameplay sounds and the game music being loaded, clip by clip
// The bar eases toward the real value but never runs ahead of it, and the game screen is only opened
// when everything is done. Back does nothing here, so the player cannot slip into a half loaded level.
public class LoadingScreen : UIScreen
{
    [SerializeField] RectTransform barFill;
    [SerializeField] TMP_Text percentText;
    [SerializeField] TMP_Text stepText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] RectTransform spinner;
    [SerializeField] MusicManager music;
    [SerializeField] float minimumSeconds = 0.9f;    // so the screen does not just flash by on a fast machine

    float real;       // how far the loading has really got
    float shown;      // what the bar shows

    public float RealProgress => real;
    public bool Finished { get; private set; }

    public override void OnOpen()
    {
        real = shown = 0f;
        Finished = false;
        levelText.text = "Sector " + (GameSession.level + 1) + "  ·  " + GameCatalog.LevelNames[GameSession.level];
        Draw();
        StartCoroutine(Load());
    }

    public override bool HandleBack() { return true; }

    IEnumerator Load()
    {
        float started = Time.unscaledTime;

        // a replay: take the old run away first
        var old = SceneManager.GetSceneByName(GameplayBridge.SceneName);
        if (old.isLoaded) yield return SceneManager.UnloadSceneAsync(old);

        stepText.text = "Loading the sector";
        AsyncOperation op = SceneManager.LoadSceneAsync(GameplayBridge.SceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            real = 0.7f * Mathf.Clamp01(op.progress / 0.9f);
            yield return null;
        }
        real = 0.7f;
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        stepText.text = "Loading sounds";
        var bridge = FindAnyObjectByType<GameplayBridge>();
        var clips = new System.Collections.Generic.List<AudioClip>(bridge.Sounds.Clips) { music.GameClip };
        for (int i = 0; i < clips.Count; i++)
        {
            clips[i].LoadAudioData();
            while (clips[i].loadState == AudioDataLoadState.Loading) yield return null;
            real = 0.7f + 0.3f * (i + 1) / clips.Count;
            yield return null;
        }

        // let the bar finish, and keep the screen up long enough to be read
        while (shown < 0.999f || Time.unscaledTime - started < minimumSeconds) yield return null;

        stepText.text = "Ready";
        Finished = true;
        yield return new WaitForSecondsRealtime(0.2f);
        ScreenManager.Instance.OpenFresh(ScreenId.Gameplay);
    }

    void Update()
    {
        shown = Mathf.MoveTowards(shown, real, Time.unscaledDeltaTime * 1.6f);
        spinner.Rotate(0f, 0f, -240f * Time.unscaledDeltaTime);
        Draw();
    }

    void Draw()
    {
        barFill.anchorMax = new Vector2(Mathf.Max(0.03f, shown), 1f);
        percentText.text = Mathf.FloorToInt(shown * 100f + 0.001f) + "%";
    }
}
