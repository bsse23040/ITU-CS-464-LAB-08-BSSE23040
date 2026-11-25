using UnityEngine;

public enum ScreenId
{
    Title, MainMenu, ModeSelect, LevelSelect, Loadout, Shop, Settings, Gameplay, Pause, Results, Credits, Loading
}

// The base class for every screen. The ScreenManager shows and hides screens and calls
// OnOpen when one starts to appear, so each screen plays its own entrance animation there.
[RequireComponent(typeof(CanvasGroup))]
public class UIScreen : MonoBehaviour
{
    public ScreenId id;

    // An overlay (the pause menu) is drawn on top of another screen instead of replacing it.
    public bool overlay;
    public ScreenId baseScreen;

    public CanvasGroup Group { get; private set; }
    public RectTransform Rect { get; private set; }

    protected virtual void Awake()
    {
        Group = GetComponent<CanvasGroup>();
        Rect = (RectTransform)transform;
    }

    public virtual void OnOpen() { }
    public virtual void OnClose() { }

    // The back button and the Escape key land here first. Return true if the screen dealt with it.
    public virtual bool HandleBack() { return false; }
}
