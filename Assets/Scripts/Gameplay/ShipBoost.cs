using System;
using UnityEngine;

// The boost button: for a few seconds the ship is shielded, everything rushes past faster and points count double.
// Then it needs to recharge.
public class ShipBoost : MonoBehaviour
{
    [SerializeField] ShipHealth health;
    [SerializeField] float seconds = 3f;
    [SerializeField] float recharge = 8f;

    public event Action<float> BoostStarted;     // how long it lasts

    public bool Active { get; private set; }
    float left, cooling;

    // 0 empty, 1 ready
    public float Charge => Active ? left / seconds : 1f - Mathf.Clamp01(cooling / recharge);
    public bool Ready => !Active && cooling <= 0f;

    public void ResetBoost()
    {
        Active = false;
        left = 0f;
        cooling = 2f;                 // a short wait at the start of a run
    }

    public bool TryActivate()
    {
        if (!Ready) return false;
        Active = true;
        left = seconds;
        health.Shielded = true;
        BoostStarted?.Invoke(seconds);
        return true;
    }

    public void Tick(float dt)
    {
        if (Active)
        {
            left -= dt;
            if (left <= 0f) Stop();
        }
        else if (cooling > 0f) cooling -= dt;
    }

    public void Stop()
    {
        if (!Active) return;
        Active = false;
        cooling = recharge;
        health.Shielded = false;
    }
}
