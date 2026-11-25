using System;
using UnityEngine;

// Everything the player owns or has chosen. It is saved as one small JSON string in PlayerPrefs,
// so the menus remember the coins, the ships, the stars and the settings between runs.
public static class PlayerData
{
    [Serializable]
    class Save
    {
        public int coins = 1000;
        public int gems = 25;
        public int pilotLevel = 4;
        public bool[] ownedShips = { true, false, false, false };
        public int equippedShip = 0;
        public int[] boosts = { 1, 0, 2, 0 };
        public int[] stars = { 3, 3, 2, 3, 1, 0, 0, 0, 0, 0, 0, 0 };
        public int[] bestScores = { 520, 455, 310, 470, 140, 0, 0, 0, 0, 0, 0, 0 };
        public float steering = 0.5f;
        public bool vibration = true;
        public bool notifications = true;
        public int quality = 1;
        public string lastGift = "";
    }

    const string Key = "comet_dash_save";
    public const int LevelCount = 12;

    static Save data;

    // the menus listen to this so the coin and gem counters update by themselves
    public static event Action Changed;

    static Save D
    {
        get
        {
            if (data == null)
            {
                if (PlayerPrefs.HasKey(Key))
                {
                    try { data = JsonUtility.FromJson<Save>(PlayerPrefs.GetString(Key)); }
                    catch (Exception) { data = null; }
                }
                if (data == null) data = new Save();
            }
            return data;
        }
    }

    static void Commit()
    {
        PlayerPrefs.SetString(Key, JsonUtility.ToJson(D));
        PlayerPrefs.Save();
        if (Changed != null) Changed();
    }

    // ---- money ----
    public static int Coins => D.coins;
    public static int Gems => D.gems;
    public static int PilotLevel => D.pilotLevel;

    public static void AddCoins(int amount)
    {
        D.coins += amount;
        Commit();
    }

    public static bool TrySpendCoins(int amount)
    {
        if (D.coins < amount) return false;
        D.coins -= amount;
        Commit();
        return true;
    }

    // ---- ships and boosts ----
    public static bool OwnsShip(int i) => D.ownedShips[i];
    public static int EquippedShip => D.equippedShip;

    public static bool BuyShip(int i)
    {
        if (D.ownedShips[i]) return true;
        if (!TrySpendCoins(GameCatalog.Ships[i].price)) return false;
        D.ownedShips[i] = true;
        Commit();
        return true;
    }

    public static void Equip(int i)
    {
        if (!D.ownedShips[i]) return;
        D.equippedShip = i;
        Commit();
    }

    public static int BoostCount(int i) => D.boosts[i];

    public static bool BuyBoost(int i)
    {
        if (!TrySpendCoins(GameCatalog.Boosts[i].price)) return false;
        D.boosts[i]++;
        Commit();
        return true;
    }

    // ---- levels ----
    public static int Stars(int level) => D.stars[level];
    public static int BestScore(int level) => D.bestScores[level];

    // the first level with no stars is the one to play next. Everything after it is locked.
    public static int HighestUnlocked
    {
        get
        {
            for (int i = 0; i < LevelCount; i++)
                if (D.stars[i] == 0) return i;
            return LevelCount - 1;
        }
    }

    public static bool IsUnlocked(int level) => level <= HighestUnlocked;

    // returns true when the score beats the old best
    public static bool SaveRun(int level, int score, int stars, int coinsEarned)
    {
        bool newBest = score > D.bestScores[level];
        if (newBest) D.bestScores[level] = score;
        if (stars > D.stars[level]) D.stars[level] = stars;
        D.coins += coinsEarned;
        Commit();
        return newBest;
    }

    // ---- daily gift ----
    public static bool GiftReady => D.lastGift != DateTime.Now.ToString("yyyyMMdd");

    public static void ClaimGift(int coins)
    {
        D.lastGift = DateTime.Now.ToString("yyyyMMdd");
        D.coins += coins;
        Commit();
    }

    // ---- settings ----
    public static float Steering { get => D.steering; set { D.steering = value; Commit(); } }
    public static bool Vibration { get => D.vibration; set { D.vibration = value; Commit(); } }
    public static bool Notifications { get => D.notifications; set { D.notifications = value; Commit(); } }
    public static int Quality { get => D.quality; set { D.quality = value; Commit(); } }

    public static void ResetProgress()
    {
        data = new Save();
        Commit();
    }
}
