using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The sector map. Tap an open tile to pick it, then Start. Locked tiles shake and say what to do first.
public class LevelSelectScreen : UIScreen
{
    [SerializeField] LevelTile[] tiles;
    [SerializeField] TMP_Text modeChip;
    [SerializeField] RectTransform infoPanel;
    [SerializeField] TMP_Text infoTitle;
    [SerializeField] TMP_Text infoBest;
    [SerializeField] Image[] infoStars;
    [SerializeField] Sprite starFull;
    [SerializeField] Sprite starEmpty;
    [SerializeField] Color starOn = Color.yellow;
    [SerializeField] Color starOff = Color.gray;
    [SerializeField] StyledButton start;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < tiles.Length; i++)
        {
            int k = i;
            tiles[i].Button.onClick.AddListener(() => Pick(k));
        }
        start.Button.onClick.AddListener(StartRun);
    }

    public override void OnOpen()
    {
        modeChip.text = GameCatalog.Modes[GameSession.mode].name.ToUpper();

        // keep the old pick if it is still open, otherwise jump to the newest sector
        if (!PlayerData.IsUnlocked(GameSession.level)) GameSession.level = PlayerData.HighestUnlocked;

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Refresh(i, i == GameSession.level);
            // the tiles pop in as a wave from the top left corner
            int row = i / 3;
            int col = i % 3;
            UIAnim.PopIn((RectTransform)tiles[i].transform, 0.05f + (row + col) * 0.05f, 0.7f);
        }
        ShowInfo();
    }

    void Pick(int i)
    {
        if (tiles[i].Locked)
        {
            Tween.Shake((RectTransform)tiles[i].transform);
            Toast.Show("Finish sector " + i + " first");
            UiFeedback.Error();
            return;
        }
        if (i == GameSession.level) return;

        tiles[GameSession.level].SetSelected(false);
        GameSession.level = i;
        tiles[i].SetSelected(true);
        ShowInfo();
        UIAnim.Punch(infoPanel, 1.03f);
    }

    void ShowInfo()
    {
        int level = GameSession.level;
        infoTitle.text = "Sector " + (level + 1) + "  ·  " + GameCatalog.LevelNames[level];
        int best = PlayerData.BestScore(level);
        infoBest.text = best > 0 ? "Best " + best : "Not played yet";
        int earned = PlayerData.Stars(level);
        for (int i = 0; i < infoStars.Length; i++)
        {
            infoStars[i].sprite = i < earned ? starFull : starEmpty;
            infoStars[i].color = i < earned ? starOn : starOff;
        }
    }

    void StartRun()
    {
        ScreenManager.Instance.Open(ScreenId.Loading);
    }
}
