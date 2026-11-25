using UnityEngine;

// The menus' sky and stars. It fades away while the game is on screen, because the game has its own.
[RequireComponent(typeof(CanvasGroup))]
public class MenuBackdrop : MonoBehaviour
{
    public static MenuBackdrop Instance { get; private set; }

    CanvasGroup group;

    void Awake()
    {
        Instance = this;
        group = GetComponent<CanvasGroup>();
    }

    public void Show(bool visible)
    {
        if (group == null) return;
        Tween.Fade(group, visible ? 1f : 0f, 0.3f, Ease.OutQuad);
    }
}
