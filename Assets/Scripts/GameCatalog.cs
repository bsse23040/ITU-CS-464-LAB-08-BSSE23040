using UnityEngine;

// The fixed lists the menus show: ships, boosts, modes and level names.
// Nothing here changes while playing. What the player owns lives in PlayerData.
public static class GameCatalog
{
    public class Ship
    {
        public string name;
        public string role;
        public int price;
        public float speed, handling, boost;   // 0 to 1, drawn as the stat bars
    }

    public class Boost
    {
        public string name;
        public string blurb;
        public int price;
    }

    public class Mode
    {
        public string name;
        public string blurb;
        public int unlockLevel;   // the level number (starting at 1) you need to reach
    }

    public static readonly Ship[] Ships =
    {
        new Ship { name = "Spark", role = "All-rounder",   price = 0,    speed = 0.50f, handling = 0.60f, boost = 0.40f },
        new Ship { name = "Nova",  role = "Fast and loose", price = 800,  speed = 0.85f, handling = 0.45f, boost = 0.55f },
        new Ship { name = "Orbit", role = "Easy to steer",  price = 1500, speed = 0.55f, handling = 0.90f, boost = 0.50f },
        new Ship { name = "Vega",  role = "Boost specialist", price = 2400, speed = 0.70f, handling = 0.65f, boost = 0.95f },
    };

    public static readonly Boost[] Boosts =
    {
        new Boost { name = "Shield",       blurb = "Absorbs one hit", price = 150 },
        new Boost { name = "Magnet",       blurb = "Pulls coins in",  price = 200 },
        new Boost { name = "Slow-mo",      blurb = "5 calm seconds",  price = 250 },
        new Boost { name = "Double score", blurb = "x2 for one run",  price = 300 },
    };

    public static readonly Mode[] Modes =
    {
        new Mode { name = "Classic",     blurb = "Fly to the portal at a steady pace.", unlockLevel = 1 },
        new Mode { name = "Time Attack", blurb = "Beat the clock for bonus stars.",      unlockLevel = 1 },
        new Mode { name = "Endless",     blurb = "No portal. See how far you get.",      unlockLevel = 8 },
    };

    public static readonly string[] LevelNames =
    {
        "Dust Belt", "Ringlight", "Cold Drift", "Ember Gap", "Glass Moon", "Slow Tide",
        "Iron Veil", "Night Bloom", "Static Sea", "Red Corridor", "Last Lantern", "The Portal"
    };

    // how many points a run needs for one, two and three stars
    public static readonly int[] StarScores = { 100, 250, 400 };

    public static int StarsFor(int score)
    {
        int stars = 0;
        for (int i = 0; i < StarScores.Length; i++)
            if (score >= StarScores[i]) stars = i + 1;
        return stars;
    }
}
