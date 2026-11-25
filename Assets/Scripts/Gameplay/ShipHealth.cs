using System;
using UnityEngine;

// The ship's hearts. It only keeps the numbers and the rules, and announces what happened as events.
// The HUD, the sounds, the sparks, the shake and the red flash are separate listeners it knows nothing about.
public class ShipHealth : MonoBehaviour
{
    [SerializeField] int max = 3;
    [SerializeField] float safeSeconds = 1.2f;      // after a hit nothing can hurt the ship for a moment

    public event Action<int, int> Damaged;          // hearts left, most there can be
    public event Action<int, int> Healed;           // hearts now, most there can be
    public event Action Died;

    public int Current { get; private set; }
    public int Max => max;
    public bool Shielded { get; set; }              // the boost protects the ship
    public bool Hurt => Time.time < safeUntil;      // blinking after a hit
    float safeUntil;

    public void Restore()
    {
        Current = max;
        safeUntil = 0f;
        Shielded = false;
    }

    // returns true when the hit counted
    public bool TakeHit(int amount = 1)
    {
        if (Current <= 0 || Shielded || Hurt) return false;
        Current = Mathf.Max(0, Current - amount);
        safeUntil = Time.time + safeSeconds;
        Damaged?.Invoke(Current, max);
        if (Current == 0) Died?.Invoke();
        return true;
    }

    public bool Heal(int amount = 1)
    {
        if (Current <= 0 || Current >= max) return false;
        Current = Mathf.Min(max, Current + amount);
        Healed?.Invoke(Current, max);
        return true;
    }
}
