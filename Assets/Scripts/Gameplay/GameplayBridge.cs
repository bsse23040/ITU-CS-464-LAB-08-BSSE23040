using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Sits in the Gameplay scene, which is loaded on top of the menu scene. It connects the two:
//   shows the game while the game or the pause menu is on screen, and hides it behind other screens
//   starts the run the first time the game is actually shown (never while it is still loading)
//   unloads the whole Gameplay scene once the player has left the run
public class GameplayBridge : MonoBehaviour
{
    [SerializeField] CanvasGroup view;
    [SerializeField] RunController run;
    [SerializeField] GameSfx sounds;

    public const string SceneName = "Gameplay";

    ScreenManager screens;
    bool unloading;

    public RunController Run => run;
    public GameSfx Sounds => sounds;

    void Awake() { SetVisible(false, false); }

    void OnEnable()
    {
        screens = ScreenManager.Instance;
        if (screens != null) screens.ScreenChanged += OnScreenChanged;
    }

    void OnDisable()
    {
        if (screens != null) screens.ScreenChanged -= OnScreenChanged;
        if (MenuBackdrop.Instance != null) MenuBackdrop.Instance.Show(true);
    }

    void OnScreenChanged(ScreenId from, ScreenId to, NavKind kind)
    {
        bool visible = to == ScreenId.Gameplay || to == ScreenId.Pause;
        SetVisible(visible, true);

        if (to == ScreenId.Gameplay && run.Phase == RunController.State.Ready)
        {
            if (SfxPlayer.Instance != null) SfxPlayer.Instance.StopAll();
            run.Begin();
        }

        // gone from the run altogether (results, main menu), and not on the way to a fresh load
        if (to != ScreenId.Loading && !screens.InStack(ScreenId.Gameplay) && !unloading)
        {
            unloading = true;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            StartCoroutine(Unload());
        }
    }

    // after the next screen has slid in, so the game fades out instead of vanishing
    IEnumerator Unload()
    {
        yield return new WaitForSecondsRealtime(0.34f);
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }

    void SetVisible(bool visible, bool animate)
    {
        view.interactable = visible;
        view.blocksRaycasts = visible;
        if (animate) Tween.Fade(view, visible ? 1f : 0f, 0.3f, Ease.OutQuad);
        else view.alpha = visible ? 1f : 0f;
        if (animate && MenuBackdrop.Instance != null) MenuBackdrop.Instance.Show(!visible);
    }
}
