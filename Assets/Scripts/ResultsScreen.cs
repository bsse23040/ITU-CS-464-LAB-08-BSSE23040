using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The end of a run. The numbers count up and the stars pop in one by one,
// so the player gets the good news in order instead of all at once.
public class ResultsScreen : UIScreen
{
    [SerializeField] RectTransform title;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text subtitle;
    [SerializeField] Image[] stars;
    [SerializeField] Sprite starFull;
    [SerializeField] Sprite starEmpty;
    [SerializeField] Color starOn = Color.yellow;
    [SerializeField] Color starOff = Color.gray;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text bestText;
    [SerializeField] RectTransform newBestBadge;
    [SerializeField] RectTransform scoreCard;
    [SerializeField] RectTransform buttonArea;
    [SerializeField] StyledButton primary;
    [SerializeField] StyledButton left;

    Vector2 titleHome;
    Vector2 buttonHome;
    bool won;

    protected override void Awake()
    {
        base.Awake();
        titleHome = title.anchoredPosition;
        buttonHome = buttonArea.anchoredPosition;
        primary.Button.onClick.AddListener(OnPrimary);
        left.Button.onClick.AddListener(OnLeft);
    }

    public override void OnOpen()
    {
        var r = GameSession.last;
        won = r.won;

        titleText.text = won ? "Sector clear!" : "Run over";
        subtitle.text = GameSession.LevelLabel;
        bool lastLevel = GameSession.level >= PlayerData.LevelCount - 1;
        primary.SetLabel(won ? (lastLevel ? "PLAY AGAIN" : "NEXT SECTOR") : "TRY AGAIN");
        left.SetLabel(won ? "RETRY" : "LOADOUT");
        bestText.text = "Best  " + PlayerData.BestScore(GameSession.level);

        // start from empty, then play it in
        scoreText.text = "0";
        coinsText.text = "+0";
        newBestBadge.localScale = Vector3.zero;

        var titleGroup = UIAnim.GroupOf(title);
        titleGroup.alpha = 0f;
        title.anchoredPosition = titleHome + new Vector2(0f, 90f);
        Tween.Move(title, titleHome, 0.55f, Ease.OutBack);
        Tween.Fade(titleGroup, 1f, 0.3f, Ease.OutQuad);

        UIAnim.PopIn(scoreCard, 0.15f, 0.92f);

        // each star pops with a springy overshoot, one after the other
        for (int i = 0; i < stars.Length; i++)
        {
            bool earned = i < r.stars;
            stars[i].sprite = earned ? starFull : starEmpty;
            stars[i].color = earned ? starOn : starOff;
            stars[i].transform.localScale = Vector3.zero;
            if (earned) Tween.Scale(stars[i].transform, 1f, 0.75f, Ease.OutElastic, 0.45f + i * 0.3f);
            else Tween.Scale(stars[i].transform, 1f, 0.3f, Ease.OutQuad, 0.45f + i * 0.3f);
        }

        // the score decelerates into its final value, the coins follow
        Tween.Count(scoreText, "count", 0, r.score, 1.2f, Ease.OutCubic, v => scoreText.text = v.ToString("N0"), 0.4f);
        Tween.Count(coinsText, "count", 0, r.coins, 0.9f, Ease.OutQuad, v => coinsText.text = "+" + v.ToString("N0"), 1.1f);

        if (r.newBest) Tween.Scale(newBestBadge, 1f, 0.5f, Ease.OutBack, 1.7f);
        else newBestBadge.localScale = Vector3.zero;

        // the buttons come last, so the eye ends up on what to do next
        UIAnim.RiseIn(buttonArea, buttonHome, 1.5f, 120f);
    }

    void OnPrimary()
    {
        if (won)
        {
            int next = GameSession.level + 1;
            GameSession.level = next >= PlayerData.LevelCount ? 0 : next;
        }
        ScreenManager.Instance.OpenFresh(ScreenId.Loading);
    }

    void OnLeft()
    {
        if (won)
        {
            ScreenManager.Instance.OpenFresh(ScreenId.Loading);
        }
        else
        {
            ScreenManager.Instance.Open(ScreenId.Loadout);
        }
    }

    public override bool HandleBack()
    {
        ScreenManager.Instance.OpenFresh(ScreenId.MainMenu);
        return true;
    }
}
