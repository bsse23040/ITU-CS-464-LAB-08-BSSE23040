using UnityEngine;
using UnityEngine.UI;

// A listener: a short coloured flash. Red for damage, green for a repair.
public class HitFlash : MonoBehaviour
{
    [SerializeField] ShipHealth health;
    [SerializeField] Image flash;
    [SerializeField] Color hurt = Color.red;
    [SerializeField] Color mend = Color.green;
    [SerializeField] float seconds = 0.35f;

    float left, strength;

    void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Healed += OnHealed;
    }

    void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Healed -= OnHealed;
    }

    void OnDamaged(int current, int max) { Show(hurt, 0.34f); }
    void OnHealed(int current, int max) { Show(mend, 0.2f); }

    void Show(Color color, float alpha)
    {
        strength = alpha;
        left = seconds;
        flash.color = new Color(color.r, color.g, color.b, alpha);
    }

    void Update()
    {
        if (left <= 0f) return;
        left -= Time.deltaTime;
        Color c = flash.color;
        c.a = strength * Mathf.Max(0f, left / seconds);
        flash.color = c;
    }
}
