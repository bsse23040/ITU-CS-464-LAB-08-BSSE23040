using UnityEngine;

// Pick how to play. One card is always picked. A locked card shakes and explains itself when tapped.
public class ModeSelectScreen : UIScreen
{
    [SerializeField] ModeCard[] cards;
    [SerializeField] StyledButton next;

    Vector2[] homes;

    protected override void Awake()
    {
        base.Awake();
        homes = new Vector2[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            int k = i;
            homes[i] = ((RectTransform)cards[i].transform).anchoredPosition;
            cards[i].Button.onClick.AddListener(() => Pick(k));
        }
    }

    public override void OnOpen()
    {
        int level = PlayerData.HighestUnlocked + 1;
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Setup(GameCatalog.Modes[i], level < GameCatalog.Modes[i].unlockLevel);
            // the cards rise into place one after the other
            UIAnim.RiseIn((RectTransform)cards[i].transform, homes[i], 0.1f + i * 0.09f, 90f);
        }

        if (cards[GameSession.mode].Locked) GameSession.mode = 0;
        cards[GameSession.mode].SetSelected(true, false);
        next.SetInteractable(true);
    }

    void Pick(int i)
    {
        if (cards[i].Locked)
        {
            Tween.Shake((RectTransform)cards[i].transform);
            Toast.Show("Reach sector " + GameCatalog.Modes[i].unlockLevel + " to unlock " + GameCatalog.Modes[i].name);
            UiFeedback.Error();
            return;
        }
        if (i == GameSession.mode) return;

        cards[GameSession.mode].SetSelected(false, true);
        GameSession.mode = i;
        cards[i].SetSelected(true, true);
    }
}
