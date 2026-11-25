using UnityEngine;

// Keeps the menus out of the notch, the rounded corners and the gesture bar.
// It squeezes this rect into Screen.safeArea. On a normal screen that is the whole screen.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    Rect applied;
    Vector2Int size;

    void Update()
    {
        if (Screen.safeArea != applied || size.x != Screen.width || size.y != Screen.height)
            Apply();
    }

    void Apply()
    {
        applied = Screen.safeArea;
        size = new Vector2Int(Screen.width, Screen.height);
        var rt = (RectTransform)transform;
        Vector2 min = applied.position;
        Vector2 max = applied.position + applied.size;
        min.x /= Screen.width; min.y /= Screen.height;
        max.x /= Screen.width; max.y /= Screen.height;
        rt.anchorMin = min;
        rt.anchorMax = max;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
