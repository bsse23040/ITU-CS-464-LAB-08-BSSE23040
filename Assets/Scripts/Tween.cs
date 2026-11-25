using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The easing curves used in the menus. Each one takes t from 0 to 1 and gives back the eased value.
// OutBack and OutElastic go past 1 for a moment, so the tweens use LerpUnclamped.
public enum Ease { Linear, InQuad, OutQuad, InOutQuad, InCubic, OutCubic, InOutCubic, OutQuart, InOutSine, OutBack, OutElastic }

public static class Easing
{
    public static float Apply(Ease ease, float t)
    {
        t = Mathf.Clamp01(t);
        switch (ease)
        {
            case Ease.InQuad: return t * t;
            case Ease.OutQuad: return 1f - (1f - t) * (1f - t);
            case Ease.InOutQuad: return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            case Ease.InCubic: return t * t * t;
            case Ease.OutCubic: return 1f - Mathf.Pow(1f - t, 3f);
            case Ease.InOutCubic: return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
            case Ease.OutQuart: return 1f - Mathf.Pow(1f - t, 4f);
            case Ease.InOutSine: return -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;
            case Ease.OutBack:
                {
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1f;
                    return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
                }
            case Ease.OutElastic:
                {
                    if (t == 0f || t == 1f) return t;
                    const float c4 = 2f * Mathf.PI / 3f;
                    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
                }
            default: return t;
        }
    }
}

// A small tween helper made of coroutines. They all run on unscaled time, so the menus still
// animate while the game is paused with Time.timeScale = 0.
// A tween is stored under (owner, slot). Starting a new tween with the same owner and slot
// cancels the old one, so two tweens never fight over the same property.
public static class Tween
{
    class Runner : MonoBehaviour { }

    static Runner runner;
    static readonly Dictionary<(object, string), int> serials = new Dictionary<(object, string), int>();
    static readonly Dictionary<RectTransform, Vector2> shakeHome = new Dictionary<RectTransform, Vector2>();
    static int counter;

    static Runner Host
    {
        get
        {
            if (runner == null)
            {
                serials.Clear();
                shakeHome.Clear();
                runner = new GameObject("Tween Runner").AddComponent<Runner>();
            }
            return runner;
        }
    }

    // ---- the general version ----

    public static void Value(object owner, string slot, float seconds, Ease ease, Action<float> update, Action done = null, float delay = 0f)
    {
        var host = Host;   // make sure the runner exists before this tween is registered
        var key = (owner, slot);
        int id = ++counter;
        serials[key] = id;
        host.StartCoroutine(Play(key, id, seconds, ease, update, done, delay, false));
    }

    // goes there and back again until something else cancels it
    public static void Loop(object owner, string slot, float seconds, Ease ease, Action<float> update)
    {
        var host = Host;
        var key = (owner, slot);
        int id = ++counter;
        serials[key] = id;
        host.StartCoroutine(Play(key, id, seconds, ease, update, null, 0f, true));
    }

    public static void Cancel(object owner, string slot)
    {
        serials.Remove((owner, slot));
    }

    static bool Alive(object owner, (object, string) key, int id)
    {
        if (owner is UnityEngine.Object o && o == null) return false;
        return serials.TryGetValue(key, out int current) && current == id;
    }

    static IEnumerator Play((object, string) key, int id, float seconds, Ease ease, Action<float> update, Action done, float delay, bool pingPong)
    {
        object owner = key.Item1;
        float waited = 0f;
        while (waited < delay)
        {
            if (!Alive(owner, key, id)) yield break;
            waited += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            yield return null;
        }

        float t = 0f;
        float dir = 1f;
        while (pingPong || t < seconds)
        {
            if (!Alive(owner, key, id)) yield break;
            t += Mathf.Min(Time.unscaledDeltaTime, 0.05f) * dir;
            if (pingPong)
            {
                if (t >= seconds) { t = seconds; dir = -1f; }
                else if (t <= 0f) { t = 0f; dir = 1f; }
            }
            update(Easing.Apply(ease, t / seconds));
            yield return null;
        }

        update(Easing.Apply(ease, 1f));
        if (serials.TryGetValue(key, out int last) && last == id) serials.Remove(key);
        if (done != null) done();
    }

    // ---- shortcuts for the things the menus move most ----

    public static void Fade(CanvasGroup group, float to, float seconds, Ease ease = Ease.OutCubic, float delay = 0f, Action done = null)
    {
        float from = group.alpha;
        Value(group, "fade", seconds, ease, k => group.alpha = Mathf.LerpUnclamped(from, to, k), done, delay);
    }

    public static void Scale(Transform target, float to, float seconds, Ease ease = Ease.OutCubic, float delay = 0f, Action done = null)
    {
        Vector3 from = target.localScale;
        Vector3 end = Vector3.one * to;
        Value(target, "scale", seconds, ease, k => target.localScale = Vector3.LerpUnclamped(from, end, k), done, delay);
    }

    public static void Move(RectTransform target, Vector2 to, float seconds, Ease ease = Ease.OutCubic, float delay = 0f, Action done = null)
    {
        Vector2 from = target.anchoredPosition;
        Value(target, "move", seconds, ease, k => target.anchoredPosition = Vector2.LerpUnclamped(from, to, k), done, delay);
    }

    public static void Tint(Graphic target, Color to, float seconds, Ease ease = Ease.OutQuad, float delay = 0f)
    {
        Color from = target.color;
        Value(target, "tint", seconds, ease, k => target.color = Color.LerpUnclamped(from, to, k), null, delay);
    }

    // numbers that count up or down
    public static void Count(object owner, string slot, float from, float to, float seconds, Ease ease, Action<int> show, float delay = 0f)
    {
        Value(owner, slot, seconds, ease, k => show(Mathf.RoundToInt(Mathf.Lerp(from, to, k))), null, delay);
    }

    // a quick side to side wiggle that dies away. Used when something is not allowed.
    public static void Shake(RectTransform target, float strength = 18f, float seconds = 0.4f)
    {
        if (!shakeHome.ContainsKey(target)) shakeHome[target] = target.anchoredPosition;
        Vector2 home = shakeHome[target];
        Value(target, "shake", seconds, Ease.Linear, k =>
        {
            float x = Mathf.Sin(k * Mathf.PI * 6f) * strength * (1f - k);
            target.anchoredPosition = home + new Vector2(x, 0f);
        }, () =>
        {
            target.anchoredPosition = home;
            shakeHome.Remove(target);
        });
    }
}
