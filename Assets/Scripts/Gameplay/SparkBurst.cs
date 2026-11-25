using UnityEngine;
using UnityEngine.UI;

// A listener: little bursts of sparks. The game is drawn on a UI canvas, where a Particle System would be
// hidden behind the canvas, so the sparks are a small pool of UI images that fly out and fade.
public class SparkBurst : MonoBehaviour
{
    [SerializeField] ShipHealth health;
    [SerializeField] FallingField field;
    [SerializeField] ShipBoost boost;
    [SerializeField] RectTransform ship;
    [SerializeField] Image[] sparks;
    [SerializeField] Color coinColor = Color.yellow;
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] Color healColor = Color.green;
    [SerializeField] Color boostColor = Color.cyan;

    Vector2[] velocity;
    float[] life, total;
    int next;

    public int Alive { get; private set; }

    void Awake()
    {
        velocity = new Vector2[sparks.Length];
        life = new float[sparks.Length];
        total = new float[sparks.Length];
        foreach (var s in sparks) s.enabled = false;
    }

    void OnEnable()
    {
        field.Collected += OnCollected;
        field.Smashed += OnSmashed;
        health.Damaged += OnDamaged;
        health.Healed += OnHealed;
        health.Died += OnDied;
        boost.BoostStarted += OnBoost;
    }

    void OnDisable()
    {
        field.Collected -= OnCollected;
        field.Smashed -= OnSmashed;
        health.Damaged -= OnDamaged;
        health.Healed -= OnHealed;
        health.Died -= OnDied;
        boost.BoostStarted -= OnBoost;
    }

    void OnCollected(PickupKind kind, Vector2 where) { Burst(where, kind == PickupKind.Coin ? coinColor : healColor, 10, 380f, 0.45f); }
    void OnSmashed(Vector2 where) { Burst(where, boostColor, 10, 520f, 0.4f); }
    void OnDamaged(int current, int max) { Burst(ship.localPosition, hitColor, 16, 560f, 0.55f); }
    void OnHealed(int current, int max) { Burst(ship.localPosition, healColor, 18, 330f, 0.7f); }
    void OnDied() { Burst(ship.localPosition, hitColor, 40, 760f, 1.0f); }
    void OnBoost(float seconds) { Burst(ship.localPosition, boostColor, 20, 620f, 0.5f); }

    void Burst(Vector2 at, Color color, int count, float speed, float seconds)
    {
        for (int n = 0; n < count; n++)
        {
            int i = next;
            next = (next + 1) % sparks.Length;
            float angle = (n + Random.value) / count * Mathf.PI * 2f;
            velocity[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed * Random.Range(0.45f, 1f);
            total[i] = life[i] = seconds * Random.Range(0.7f, 1.1f);
            sparks[i].rectTransform.localPosition = at;
            sparks[i].color = color;
            sparks[i].enabled = true;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        int alive = 0;
        for (int i = 0; i < sparks.Length; i++)
        {
            if (life[i] <= 0f) continue;
            life[i] -= dt;
            if (life[i] <= 0f) { sparks[i].enabled = false; continue; }
            alive++;
            velocity[i] *= 1f - 2.5f * dt;
            sparks[i].rectTransform.localPosition += (Vector3)(velocity[i] * dt);
            float left = life[i] / total[i];
            Color c = sparks[i].color;
            c.a = left;
            sparks[i].color = c;
            sparks[i].rectTransform.localScale = Vector3.one * Mathf.Lerp(0.3f, 1f, left);
        }
        Alive = alive;
    }
}
