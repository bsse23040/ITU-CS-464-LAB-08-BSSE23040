using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The pause menu is an overlay: the game stays visible and dimmed behind it, and Time.timeScale is 0
// until the player resumes. The dark layer fades in while the panel pops in with a small overshoot.
// AudioListener.pause stops the game sounds mid-note. The interface sounds and the music are set to
// ignore it, so the menu still clicks and the music carries on, quieter (see MusicManager).
public class PauseScreen : UIScreen
{
    [SerializeField] Image dim;
    [SerializeField] RectTransform panel;
    [SerializeField] TMP_Text subtitle;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] RectTransform[] buttons;
    [SerializeField] Button resume;
    [SerializeField] Button restart;
    [SerializeField] Button quit;
    [SerializeField] float dimAlpha = 0.72f;

    protected override void Awake()
    {
        base.Awake();
        resume.onClick.AddListener(Resume);
        restart.onClick.AddListener(Restart);
        quit.onClick.AddListener(Quit);
    }

    public override void OnOpen()
    {
        SetPaused(true);
        subtitle.text = GameSession.LevelLabel;
        var run = RunController.Current;
        scoreText.text = (run != null ? run.Score : 0).ToString("N0");

        var c = dim.color;
        c.a = 0f;
        dim.color = c;
        c.a = dimAlpha;
        Tween.Tint(dim, c, 0.25f, Ease.OutQuad);

        panel.localScale = Vector3.one * 0.82f;
        Tween.Scale(panel, 1f, 0.38f, Ease.OutBack);

        for (int i = 0; i < buttons.Length; i++)
            UIAnim.PopIn(buttons[i], 0.12f + i * 0.06f, 0.9f);
    }

    static void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    void Resume()
    {
        SetPaused(false);
        ScreenManager.Instance.GoBack();
    }

    // a restart goes through the loading screen, which builds a brand new Gameplay scene
    void Restart()
    {
        SetPaused(false);
        ScreenManager.Instance.OpenFresh(ScreenId.Loading);
    }

    void Quit()
    {
        SetPaused(false);
        ScreenManager.Instance.OpenFresh(ScreenId.MainMenu);
    }

    public override bool HandleBack()
    {
        Resume();
        return true;
    }
}
