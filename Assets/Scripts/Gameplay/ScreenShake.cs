using UnityEngine;

// A listener: shakes the play field when the ship is hit. The HUD sits outside it, so it stays still and readable.
public class ScreenShake : MonoBehaviour
{
    [SerializeField] ShipHealth health;
    [SerializeField] RectTransform shaken;
    [SerializeField] float hitStrength = 28f;

    float strength, seconds, left;

    void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Died -= OnDied;
    }

    void OnDamaged(int current, int max) { Shake(hitStrength, 0.35f); }
    void OnDied() { Shake(hitStrength * 2f, 0.7f); }

    void Shake(float s, float t)
    {
        strength = s;
        seconds = left = t;
    }

    void Update()
    {
        if (left <= 0f) return;
        left -= Time.deltaTime;
        float k = Mathf.Max(0f, left / seconds);
        shaken.anchoredPosition = left > 0f ? Random.insideUnitCircle * strength * k : Vector2.zero;
    }
}
