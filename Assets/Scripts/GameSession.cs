using System;

// What the player picked on the way to a run, and how the last run went.
// The menu screens read these instead of passing values around.
public static class GameSession
{
    public class Result
    {
        public bool won;
        public int score;
        public int stars;
        public int coins;
        public bool newBest;
    }

    public static int mode;          // index into GameCatalog.Modes
    public static int level;         // 0 to 11
    public static int loadoutView = -1;   // the ship the loadout was showing when it sent the player to the shop
    public static Result last = new Result();

    public static string LevelLabel => "Sector " + (level + 1) + "  ·  " + GameCatalog.Modes[mode].name;

    // Called when a run ends. Reaching the portal wins the sector with at least one star,
    // and a bigger score earns more. The coins picked up on the way are kept either way.
    public static Result Finish(int score, bool reachedPortal, int coinsPicked)
    {
        var r = new Result();
        r.score = score;
        r.won = reachedPortal;
        r.stars = reachedPortal ? Math.Max(1, GameCatalog.StarsFor(score)) : 0;
        r.coins = coinsPicked * 5 + r.stars * 10;
        r.newBest = PlayerData.SaveRun(level, score, r.stars, r.coins);
        last = r;
        return r;
    }
}
