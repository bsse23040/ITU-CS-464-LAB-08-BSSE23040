using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Knows every screen, remembers where the player came from, and plays the transitions.
//
//   Open(id)       goes forward. The new screen slides in from the right and the old one slides out to the left.
//   Back()         goes back along the history. The same slide, the other way round.
//   OpenFresh(id)  forgets the history. Used for "Menu" buttons and after quitting a run.
//
// Every change of screen is announced with the ScreenChanged event, so the sounds, the music and the
// game can react without the ScreenManager knowing about any of them.
//
// Overlays (the pause menu) fade and scale in over the screen below them, which stays visible.
public enum NavKind { Open, Back, Fresh }

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [SerializeField] UIScreen[] screens;
    [SerializeField] ScreenId first = ScreenId.Title;
    [SerializeField] RectTransform slideRoot;     // used to know how wide the screen is
    [SerializeField] Image inputBlocker;          // a see-through image that stops taps while a transition runs
    [SerializeField] float slideTime = 0.38f;

    readonly Stack<ScreenId> history = new Stack<ScreenId>();
    readonly List<UIScreen> active = new List<UIScreen>();
    ScreenId current;
    bool busy;

    public ScreenId Current => current;

    public event Action<ScreenId, ScreenId, NavKind> ScreenChanged;     // from, to, how

    // true while the screen is the current one or somewhere behind it (Back would reach it)
    public bool InStack(ScreenId id) => current == id || history.Contains(id);

    void Awake()
    {
        Instance = this;
        // Awake of each screen runs when it is switched on once, then they all start hidden
        foreach (var s in screens)
        {
            s.gameObject.SetActive(true);
            s.gameObject.SetActive(false);
        }
        inputBlocker.raycastTarget = false;
    }

    void Start()
    {
        current = first;
        var screen = Get(first);
        screen.gameObject.SetActive(true);
        active.Add(screen);
        screen.OnOpen();
        ScreenChanged?.Invoke(current, current, NavKind.Fresh);
    }

    void Update()
    {
        // Escape is the Android back button, and it is the same as tapping the back arrow
        if (Input.GetKeyDown(KeyCode.Escape)) Back();
    }

    UIScreen Get(ScreenId id)
    {
        foreach (var s in screens)
            if (s.id == id) return s;
        return null;
    }

    // buttons in the scene call this one, because a button can only pass an int
    public void OpenScreen(int id)
    {
        Open((ScreenId)id);
    }

    public void Open(ScreenId id)
    {
        if (busy) return;
        history.Push(current);
        Show(id, NavKind.Open);
    }

    public void OpenFresh(ScreenId id)
    {
        if (busy) return;
        history.Clear();
        // the main menu always has the title behind it, so Back there still goes somewhere
        if (id == ScreenId.MainMenu) history.Push(ScreenId.Title);
        Show(id, NavKind.Fresh);
    }

    // the back arrow and the Escape key: the current screen gets the first say
    public void Back()
    {
        if (busy) return;
        if (Get(current).HandleBack()) return;
        GoBack();
    }

    // goes back along the history without asking the screen. A screen that handles Back itself
    // (the pause menu, to resume) uses this, otherwise it would ask itself again and again.
    public void GoBack()
    {
        if (busy) return;
        if (history.Count == 0) return;
        Show(history.Pop(), NavKind.Back);
    }

    void Show(ScreenId id, NavKind kind)
    {
        ScreenId from = current;
        current = id;
        StartCoroutine(Switch(Get(id), kind != NavKind.Back));
        ScreenChanged?.Invoke(from, id, kind);
    }

    IEnumerator Switch(UIScreen to, bool forward)
    {
        busy = true;
        inputBlocker.raycastTarget = true;
        float width = slideRoot.rect.width;

        // everything on screen leaves, except the screen an overlay sits on
        var leaving = new List<UIScreen>();
        foreach (var s in active)
        {
            bool underOverlay = to.overlay && s.id == to.baseScreen;
            if (s != to && !underOverlay) leaving.Add(s);
        }
        foreach (var s in leaving) SlideOut(s, forward, width);

        if (!active.Contains(to))
        {
            if (to.overlay)
            {
                var below = Get(to.baseScreen);
                if (!active.Contains(below)) Reveal(below);
            }
            SlideIn(to, forward, width);
            to.OnOpen();
        }

        yield return new WaitForSecondsRealtime(slideTime);

        foreach (var s in leaving)
        {
            s.OnClose();
            s.gameObject.SetActive(false);
            active.Remove(s);
        }
        inputBlocker.raycastTarget = false;
        busy = false;
    }

    // puts a screen on without any animation (the gameplay screen under a pause menu)
    void Reveal(UIScreen s)
    {
        s.gameObject.SetActive(true);
        s.Group.alpha = 1f;
        s.Rect.anchoredPosition = Vector2.zero;
        s.Rect.localScale = Vector3.one;
        active.Add(s);
    }

    void SlideIn(UIScreen s, bool forward, float width)
    {
        s.gameObject.SetActive(true);
        active.Add(s);

        if (s.overlay)
        {
            s.Rect.anchoredPosition = Vector2.zero;
            s.Group.alpha = 0f;
            Tween.Fade(s.Group, 1f, 0.2f, Ease.OutQuad);
            return;
        }

        // new screens come from the right when going forward and from the left when going back
        float side = forward ? 1f : -1f;
        s.Rect.localScale = Vector3.one;
        s.Rect.anchoredPosition = new Vector2(width * 0.35f * side, 0f);
        s.Group.alpha = 0f;
        Tween.Move(s.Rect, Vector2.zero, slideTime, Ease.OutCubic);
        Tween.Fade(s.Group, 1f, slideTime * 0.8f, Ease.OutCubic);
    }

    void SlideOut(UIScreen s, bool forward, float width)
    {
        if (s.overlay)
        {
            Tween.Fade(s.Group, 0f, 0.2f, Ease.InQuad);
            return;
        }

        // old screens move less than new ones and fade, so the new one reads as being on top
        float side = forward ? -1f : 1f;
        Tween.Move(s.Rect, new Vector2(width * 0.25f * side, 0f), slideTime, Ease.InOutCubic);
        Tween.Fade(s.Group, 0f, slideTime * 0.8f, Ease.InCubic);
    }
}
