using TMPro;
using UnityEngine;

// Spend coins on ships and boosts, and claim the daily gift.
// A good purchase makes the card bump and the coin counter count down.
// A bad one shakes the card and says why.
public class ShopScreen : UIScreen
{
    [SerializeField] SegmentedControl tabs;
    [SerializeField] CanvasGroup shipsGroup;
    [SerializeField] CanvasGroup boostsGroup;
    [SerializeField] ShopItem[] shipItems;
    [SerializeField] ShopItem[] boostItems;
    [SerializeField] StyledButton giftButton;
    [SerializeField] TMP_Text giftText;
    [SerializeField] RectTransform[] popItems;
    [SerializeField] int giftCoins = 100;

    Vector2 shipsHome;
    Vector2 boostsHome;

    protected override void Awake()
    {
        base.Awake();
        shipsHome = ((RectTransform)shipsGroup.transform).anchoredPosition;
        boostsHome = ((RectTransform)boostsGroup.transform).anchoredPosition;
        tabs.Changed += ShowTab;
        foreach (var item in shipItems) Hook(item);
        foreach (var item in boostItems) Hook(item);
        giftButton.Button.onClick.AddListener(ClaimGift);
    }

    void Hook(ShopItem item)
    {
        item.Buy.Button.onClick.AddListener(() => TryBuy(item));
    }

    public override void OnOpen()
    {
        tabs.Select(0, false);
        SetTabVisible(shipsGroup, true);
        SetTabVisible(boostsGroup, false);
        ((RectTransform)shipsGroup.transform).anchoredPosition = shipsHome;
        ((RectTransform)boostsGroup.transform).anchoredPosition = boostsHome;
        RefreshAll();
        for (int i = 0; i < popItems.Length; i++)
            UIAnim.PopIn(popItems[i], 0.05f + i * 0.07f);
    }

    void RefreshAll()
    {
        foreach (var item in shipItems) item.Refresh();
        foreach (var item in boostItems) item.Refresh();

        bool ready = PlayerData.GiftReady;
        giftButton.SetInteractable(ready);
        giftButton.SetLabel(ready ? "CLAIM +" + giftCoins : "TOMORROW");
        giftText.text = ready ? "Your daily gift is ready" : "Come back tomorrow for more";
    }

    // the tab that is leaving slides a little and fades, the new one slides in from the same side
    void ShowTab(int index)
    {
        var showing = index == 0 ? shipsGroup : boostsGroup;
        var hiding = index == 0 ? boostsGroup : shipsGroup;
        float side = index == 0 ? -1f : 1f;
        Vector2 showingHome = index == 0 ? shipsHome : boostsHome;
        Vector2 hidingHome = index == 0 ? boostsHome : shipsHome;

        Tween.Move((RectTransform)hiding.transform, hidingHome + new Vector2(-side * 120f, 0f), 0.2f, Ease.InCubic);
        Tween.Fade(hiding, 0f, 0.2f, Ease.InCubic, 0f, () => SetTabVisible(hiding, false));

        showing.gameObject.SetActive(true);
        SetTabVisible(showing, true);
        showing.alpha = 0f;
        ((RectTransform)showing.transform).anchoredPosition = showingHome + new Vector2(side * 120f, 0f);
        Tween.Move((RectTransform)showing.transform, showingHome, 0.32f, Ease.OutCubic);
        Tween.Fade(showing, 1f, 0.25f, Ease.OutQuad);
    }

    void SetTabVisible(CanvasGroup group, bool visible)
    {
        group.gameObject.SetActive(visible);
        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }

    void TryBuy(ShopItem item)
    {
        var card = (RectTransform)item.transform;
        bool ok = item.IsShip ? PlayerData.BuyShip(item.Index) : PlayerData.BuyBoost(item.Index);
        if (ok)
        {
            item.Refresh();
            UIAnim.Punch(card, 1.06f);
            string bought = item.IsShip ? GameCatalog.Ships[item.Index].name : GameCatalog.Boosts[item.Index].name;
            Toast.Show(bought + (item.IsShip ? " unlocked" : " added"));
            UiFeedback.Success();
        }
        else
        {
            Tween.Shake(card);
            Toast.Show("Not enough coins");
            UiFeedback.Error();
        }
    }

    void ClaimGift()
    {
        if (!PlayerData.GiftReady) return;
        PlayerData.ClaimGift(giftCoins);
        UIAnim.Punch(giftButton.transform, 1.1f);
        Toast.Show("+" + giftCoins + " coins");
        UiFeedback.Success();
        RefreshAll();
    }
}
