using UnityEngine;

// A listener: turns gameplay events into sounds. It plays them through SfxPlayer on the SFX mixer group.
public class GameSfx : MonoBehaviour
{
    [SerializeField] RunController run;
    [SerializeField] ShipHealth health;
    [SerializeField] ShipBoost boost;
    [SerializeField] FallingField field;

    [SerializeField] AudioClip coin;
    [SerializeField] AudioClip hit;
    [SerializeField] AudioClip heal;
    [SerializeField] AudioClip boostClip;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip win;
    [SerializeField] AudioClip lose;

    // the loading screen loads the audio data of these before the run is allowed to start
    public AudioClip[] Clips => new[] { coin, hit, heal, boostClip, death, win, lose };

    void OnEnable()
    {
        field.Collected += OnCollected;
        field.Smashed += OnSmashed;
        health.Damaged += OnDamaged;
        health.Healed += OnHealed;
        health.Died += OnDied;
        boost.BoostStarted += OnBoost;
        run.GameWon += OnWon;
        run.GameLost += OnLost;
    }

    void OnDisable()
    {
        field.Collected -= OnCollected;
        field.Smashed -= OnSmashed;
        health.Damaged -= OnDamaged;
        health.Healed -= OnHealed;
        health.Died -= OnDied;
        boost.BoostStarted -= OnBoost;
        run.GameWon -= OnWon;
        run.GameLost -= OnLost;
    }

    static void Play(AudioClip clip, float jitter = 0f, float volume = 1f)
    {
        if (SfxPlayer.Instance != null) SfxPlayer.Instance.Play(clip, jitter, volume);
    }

    // coins and hits repeat a lot, so their pitch varies a little every time
    void OnCollected(PickupKind kind, Vector2 where)
    {
        if (kind == PickupKind.Coin) Play(coin, 0.09f);
    }

    void OnSmashed(Vector2 where) { Play(hit, 0.2f, 0.55f); }
    void OnDamaged(int current, int max) { if (current > 0) Play(hit, 0.08f); }
    void OnHealed(int current, int max) { Play(heal); }
    void OnDied() { Play(death); }
    void OnBoost(float seconds) { Play(boostClip, 0.04f); }
    void OnWon(int score) { Play(win); }
    void OnLost(int score) { Play(lose, 0f, 0.8f); }
}
