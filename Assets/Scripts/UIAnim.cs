using UnityEngine;

// The entrance animations that several screens share.
public static class UIAnim
{
    public static CanvasGroup GroupOf(Component c)
    {
        var g = c.GetComponent<CanvasGroup>();
        if (g == null) g = c.gameObject.AddComponent<CanvasGroup>();
        return g;
    }

    // grows from a little smaller and fades in, with a small overshoot at the end
    public static void PopIn(RectTransform r, float delay, float from = 0.85f)
    {
        var g = GroupOf(r);
        g.alpha = 0f;
        r.localScale = Vector3.one * from;
        Tween.Scale(r, 1f, 0.4f, Ease.OutBack, delay);
        Tween.Fade(g, 1f, 0.25f, Ease.OutQuad, delay);
    }

    // rises into place from below. Pass the position it should end on.
    public static void RiseIn(RectTransform r, Vector2 home, float delay, float rise = 70f)
    {
        var g = GroupOf(r);
        g.alpha = 0f;
        r.anchoredPosition = home + new Vector2(0f, -rise);
        Tween.Move(r, home, 0.45f, Ease.OutCubic, delay);
        Tween.Fade(g, 1f, 0.3f, Ease.OutQuad, delay);
    }

    // a quick bump to say "this just changed"
    public static void Punch(Transform t, float size = 1.12f)
    {
        t.localScale = Vector3.one * size;
        Tween.Scale(t, 1f, 0.35f, Ease.OutBack);
    }
}
