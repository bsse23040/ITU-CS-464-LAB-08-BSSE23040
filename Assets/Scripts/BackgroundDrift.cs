using UnityEngine;
using UnityEngine.UI;

// The slow starfield behind every screen. Each star drifts down at its own speed
// (the big ones are faster, so the field has some depth) and twinkles a little.
// It uses scaled time, so it freezes together with the game when the pause menu is open.
public class BackgroundDrift : MonoBehaviour
{
    [SerializeField] RectTransform[] stars;
    [SerializeField] Image[] starImages;
    [SerializeField] float baseSpeed = 14f;

    float[] speeds;
    float[] phases;

    void Start()
    {
        speeds = new float[stars.Length];
        phases = new float[stars.Length];
        for (int i = 0; i < stars.Length; i++)
        {
            speeds[i] = baseSpeed * (0.4f + stars[i].sizeDelta.x / 10f);
            phases[i] = i * 1.7f;
        }
    }

    void Update()
    {
        float height = ((RectTransform)transform).rect.height;
        for (int i = 0; i < stars.Length; i++)
        {
            Vector2 p = stars[i].anchoredPosition;
            p.y -= speeds[i] * Time.deltaTime;
            if (p.y < -height - 20f) p.y = 20f;
            stars[i].anchoredPosition = p;

            Color c = starImages[i].color;
            c.a = 0.45f + 0.35f * Mathf.Sin(Time.time * 1.3f + phases[i]);
            starImages[i].color = c;
        }
    }
}
