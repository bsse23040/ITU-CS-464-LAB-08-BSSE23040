using System;
using UnityEngine;

// One run through a sector. It owns the score and the clock, listens to the ship and the field,
// and announces the start and the end of a run (won at the portal, or lost with no hearts left).
public class RunController : MonoBehaviour
{
    public static RunController Current { get; private set; }

    [SerializeField] ShipHealth health;
    [SerializeField] FallingField field;
    [SerializeField] ShipBoost boost;
    [SerializeField] ShipPilot pilot;
    [SerializeField] float classicSeconds = 30f;       // how long it takes to reach the portal
    [SerializeField] float timeAttackSeconds = 22f;
    [SerializeField] float pointsPerSecond = 10f;
    [SerializeField] int pointsPerCoin = 25;

    public event Action GameStarted;
    public event Action<int> ScoreChanged;
    public event Action<int> CoinsChanged;
    public event Action<float> ProgressChanged;        // 0 at the start, 1 at the portal
    public event Action<int> GameWon;                  // the final score
    public event Action<int> GameLost;

    public enum State { Ready, Running, Over }
    public State Phase { get; private set; }
    public int Score { get; private set; }
    public int Coins { get; private set; }

    float points, elapsed, duration, modeSpeed;

    public bool HasPortal { get; private set; }       // Endless mode has none: it only ends with the last heart

    void Awake() { Current = this; }

    void OnDestroy() { if (Current == this) Current = null; }

    void OnEnable()
    {
        health.Died += OnDied;
        field.Collected += OnCollected;
    }

    void OnDisable()
    {
        health.Died -= OnDied;
        field.Collected -= OnCollected;
    }

    public void Begin()
    {
        bool timeAttack = GameSession.mode == 1;
        duration = timeAttack ? timeAttackSeconds : classicSeconds;
        HasPortal = GameSession.mode != 2;
        modeSpeed = timeAttack ? 1.25f : 1f;
        points = 0f;
        elapsed = 0f;
        Score = 0;
        Coins = 0;
        health.Restore();
        boost.ResetBoost();
        field.Clear();
        pilot.ResetShip();
        Phase = State.Running;
        GameStarted?.Invoke();
        ScoreChanged?.Invoke(0);
        CoinsChanged?.Invoke(0);
        ProgressChanged?.Invoke(0f);
    }

    // scaled time on purpose: Time.timeScale = 0 (the pause menu) stops the whole run
    void Update()
    {
        if (Phase != State.Running) return;
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        elapsed += dt;
        AddPoints(dt * pointsPerSecond);
        boost.Tick(dt);
        pilot.Tick(dt);
        field.SpeedScale = modeSpeed * (boost.Active ? 1.7f : 1f);
        field.Tick(dt);
        if (Phase != State.Running) return;            // the field may just have ended the run

        float progress = HasPortal ? Mathf.Clamp01(elapsed / duration) : 0f;
        ProgressChanged?.Invoke(progress);
        if (progress >= 1f) End(true);
    }

    void AddPoints(float amount)
    {
        points += amount * (boost.Active ? 2f : 1f);
        int score = Mathf.FloorToInt(points);
        if (score == Score) return;
        Score = score;
        ScoreChanged?.Invoke(Score);
    }

    void OnCollected(PickupKind kind, Vector2 where)
    {
        if (kind == PickupKind.Coin)
        {
            Coins++;
            CoinsChanged?.Invoke(Coins);
            AddPoints(pointsPerCoin);
        }
        else
        {
            health.Heal();
        }
    }

    void OnDied() { End(false); }

    void End(bool won)
    {
        if (Phase != State.Running) return;
        Phase = State.Over;
        boost.Stop();
        if (won) GameWon?.Invoke(Score);
        else GameLost?.Invoke(Score);
    }
}
