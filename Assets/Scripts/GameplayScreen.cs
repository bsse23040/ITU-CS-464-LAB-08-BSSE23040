using UnityEngine;

// The Gameplay step of the menu flow. The game itself lives in its own scene (Gameplay), which the
// loading screen loads on top of this one. This screen draws nothing: it is here so the ScreenManager
// knows when the player is in the game, and what Back should do there.
public class GameplayScreen : UIScreen
{
    public override bool HandleBack()
    {
        OpenPause();
        return true;
    }

    void OpenPause()
    {
        // once the run is over the results are on their way, so there is nothing to pause
        var run = RunController.Current;
        if (run != null && run.Phase == RunController.State.Running)
            ScreenManager.Instance.Open(ScreenId.Pause);
    }

    // a phone that goes to the home screen should not keep running the level
    void OnApplicationPause(bool paused)
    {
        if (paused && ScreenManager.Instance != null && ScreenManager.Instance.Current == ScreenId.Gameplay)
            OpenPause();
    }
}
