using UnityEngine;
using UnityEngine.UI;

// The credits roll upward by themselves at a steady speed, like film credits.
// Touching the screen stops the roll so you can scroll with your finger, and it carries on after a pause.
public class CreditsScreen : UIScreen
{
    [SerializeField] ScrollRect scroll;
    [SerializeField] float pixelsPerSecond = 90f;
    [SerializeField] float restartAfter = 1.5f;

    float idle;
    bool rolling;

    public override void OnOpen()
    {
        scroll.verticalNormalizedPosition = 1f;
        idle = restartAfter - 0.8f;   // a short wait, then it starts
        rolling = false;
    }

    public override void OnClose()
    {
        Tween.Cancel(scroll, "roll");
        rolling = false;
    }

    void Update()
    {
        bool touching = Input.GetMouseButton(0) || Input.touchCount > 0;
        if (touching)
        {
            Tween.Cancel(scroll, "roll");
            rolling = false;
            idle = 0f;
            return;
        }

        idle += Time.unscaledDeltaTime;
        if (rolling || idle < restartAfter) return;

        float travel = scroll.content.rect.height - scroll.viewport.rect.height;
        float from = scroll.content.anchoredPosition.y;
        if (from >= travel - 1f) return;

        // Linear on purpose: a constant speed is what makes credits easy to read
        rolling = true;
        Tween.Value(scroll, "roll", (travel - from) / pixelsPerSecond, Ease.Linear, k =>
        {
            Vector2 p = scroll.content.anchoredPosition;
            p.y = Mathf.Lerp(from, travel, k);
            scroll.content.anchoredPosition = p;
        }, () => rolling = false);
    }
}
