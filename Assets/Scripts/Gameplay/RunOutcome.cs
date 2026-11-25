using System.Collections;
using UnityEngine;

// A listener for the end of a run: saves the result, then opens the results screen once the player
// has had a moment to see and hear how it ended.
public class RunOutcome : MonoBehaviour
{
    [SerializeField] RunController run;
    [SerializeField] float delay = 1.6f;

    void OnEnable()
    {
        run.GameWon += OnWon;
        run.GameLost += OnLost;
    }

    void OnDisable()
    {
        run.GameWon -= OnWon;
        run.GameLost -= OnLost;
    }

    void OnWon(int score) { StartCoroutine(Show(score, true)); }

    void OnLost(int score)
    {
        if (PlayerData.Vibration) Handheld.Vibrate();     // does nothing on a computer
        StartCoroutine(Show(score, false));
    }

    IEnumerator Show(int score, bool reachedPortal)
    {
        GameSession.Finish(score, reachedPortal, run.Coins);
        yield return new WaitForSeconds(delay);
        ScreenManager.Instance.OpenFresh(ScreenId.Results);
    }
}
