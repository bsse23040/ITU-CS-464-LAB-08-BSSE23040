using System;
using UnityEngine;

public enum PickupKind { Coin, Repair }

// Everything that falls: rocks that hurt, coins, and a repair kit that shows up when the ship is damaged.
// Rocks tell ShipHealth directly. Pickups are announced with the Collected event, and the others decide what it means.
// The objects are made once and reused (a pool), so nothing is created or destroyed during a run.
public class FallingField : MonoBehaviour
{
    [SerializeField] RectTransform area;
    [SerializeField] RectTransform ship;
    [SerializeField] ShipHealth health;
    [SerializeField] RectTransform[] rocks;
    [SerializeField] RectTransform[] coins;
    [SerializeField] RectTransform repairKit;

    [SerializeField] float rockEvery = 0.62f;
    [SerializeField] float coinEvery = 0.75f;
    [SerializeField] float repairEvery = 7f;
    [SerializeField] float shipRadius = 70f;

    public event Action<PickupKind, Vector2> Collected;   // what, and where it was
    public event Action<Vector2> Smashed;                  // a boosting ship broke a rock

    public float SpeedScale { get; set; } = 1f;

    float[] rockSpeed, coinSpeed;
    float repairSpeed;
    float nextRock, nextCoin, nextRepair;

    void Awake()
    {
        rockSpeed = new float[rocks.Length];
        coinSpeed = new float[coins.Length];
        Clear();
    }

    public void Clear()
    {
        foreach (var r in rocks) r.gameObject.SetActive(false);
        foreach (var c in coins) c.gameObject.SetActive(false);
        repairKit.gameObject.SetActive(false);
        nextRock = 1.0f;
        nextCoin = 0.4f;
        nextRepair = repairEvery;
    }

    public void Tick(float dt)
    {
        nextRock -= dt * SpeedScale;
        if (nextRock <= 0f) { Spawn(rocks, rockSpeed, UnityEngine.Random.Range(430f, 700f)); nextRock = rockEvery; }
        nextCoin -= dt * SpeedScale;
        if (nextCoin <= 0f) { Spawn(coins, coinSpeed, 430f); nextCoin = coinEvery; }

        if (!repairKit.gameObject.activeSelf && health.Current < health.Max)
        {
            nextRepair -= dt;
            if (nextRepair <= 0f)
            {
                Put(repairKit);
                repairSpeed = 380f;
                nextRepair = repairEvery;
            }
        }

        MoveAll(rocks, rockSpeed, dt, 90f, OnRock);
        MoveAll(coins, coinSpeed, dt, 0f, r => Take(r, PickupKind.Coin));
        if (repairKit.gameObject.activeSelf && Move(repairKit, repairSpeed, dt, 0f)) Take(repairKit, PickupKind.Repair);
    }

    void Spawn(RectTransform[] pool, float[] speeds, float speed)
    {
        for (int i = 0; i < pool.Length; i++)
        {
            if (pool[i].gameObject.activeSelf) continue;
            Put(pool[i]);
            speeds[i] = speed;
            return;
        }
    }

    void Put(RectTransform item)
    {
        float half = area.rect.width / 2f - 90f;
        item.localPosition = new Vector3(UnityEngine.Random.Range(-half, half), area.rect.height / 2f + 110f, 0f);
        item.gameObject.SetActive(true);
    }

    void MoveAll(RectTransform[] pool, float[] speeds, float dt, float spin, Action<RectTransform> touched)
    {
        for (int i = 0; i < pool.Length; i++)
            if (pool[i].gameObject.activeSelf && Move(pool[i], speeds[i], dt, spin))
                touched(pool[i]);
    }

    // moves one item down and says whether it is touching the ship
    bool Move(RectTransform item, float speed, float dt, float spin)
    {
        Vector3 p = item.localPosition;
        p.y -= speed * SpeedScale * dt;
        item.localPosition = p;
        if (spin != 0f) item.localRotation = Quaternion.Euler(0f, 0f, item.localEulerAngles.z + spin * dt);
        if (p.y < -area.rect.height / 2f - 140f)
        {
            item.gameObject.SetActive(false);
            return false;
        }
        float reach = shipRadius + item.rect.width * 0.38f;
        return ((Vector2)(p - ship.localPosition)).sqrMagnitude < reach * reach;
    }

    void OnRock(RectTransform rock)
    {
        if (health.Shielded)
        {
            Smashed?.Invoke(rock.localPosition);
            rock.gameObject.SetActive(false);
        }
        else if (health.TakeHit())
        {
            rock.gameObject.SetActive(false);
        }
    }

    void Take(RectTransform item, PickupKind kind)
    {
        Vector2 where = item.localPosition;
        item.gameObject.SetActive(false);
        Collected?.Invoke(kind, where);
    }
}
