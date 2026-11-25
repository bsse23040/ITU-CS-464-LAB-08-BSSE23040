using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Browse the ships with the arrows or by swiping, and equip one. A ship you do not own
// shows its price and sends you to the shop instead of equipping.
public class LoadoutScreen : UIScreen
{
    [SerializeField] RectTransform stage;
    [SerializeField] RectTransform shipRect;
    [SerializeField] Image shipImage;
    [SerializeField] Sprite[] shipSprites;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text roleText;
    [SerializeField] Image[] dots;
    [SerializeField] RectTransform[] statFills;   // Speed, Handling, Boost. Each bar is as wide as its value.
    [SerializeField] Button previous;
    [SerializeField] Button next;
    [SerializeField] SwipeDetector swipe;
    [SerializeField] StyledButton action;
    [SerializeField] Sprite tickSprite;
    [SerializeField] Sprite lockSprite;
    [SerializeField] Color dotOn = Color.white;
    [SerializeField] Color dotOff = Color.gray;
    [SerializeField] RectTransform[] popItems;

    int viewing;
    bool moving;

    protected override void Awake()
    {
        base.Awake();
        previous.onClick.AddListener(() => Browse(-1));
        next.onClick.AddListener(() => Browse(1));
        swipe.Swiped += Browse;
        action.Button.onClick.AddListener(OnAction);
    }

    public override void OnOpen()
    {
        // coming back from the shop, show the ship that was being looked at, not the equipped one
        viewing = GameSession.loadoutView >= 0 ? GameSession.loadoutView : PlayerData.EquippedShip;
        GameSession.loadoutView = -1;
        moving = false;
        Show(false);
        for (int i = 0; i < popItems.Length; i++)
            UIAnim.PopIn(popItems[i], 0.05f + i * 0.07f);
    }

    // the ship slides out one side and the next one slides in from the other
    void Browse(int step)
    {
        if (moving) return;
        int count = GameCatalog.Ships.Length;
        int target = (viewing + step + count) % count;
        moving = true;

        var group = UIAnim.GroupOf(shipRect);
        Tween.Move(shipRect, new Vector2(-step * 220f, 0f), 0.16f, Ease.InCubic);
        Tween.Fade(group, 0f, 0.16f, Ease.InCubic, 0f, () =>
        {
            viewing = target;
            Show(true);
            shipRect.anchoredPosition = new Vector2(step * 220f, 0f);
            Tween.Move(shipRect, Vector2.zero, 0.3f, Ease.OutCubic);
            Tween.Fade(group, 1f, 0.2f, Ease.OutQuad, 0f, () => moving = false);
        });
    }

    void Show(bool animateStats)
    {
        var ship = GameCatalog.Ships[viewing];
        shipImage.sprite = shipSprites[viewing];
        nameText.text = ship.name;
        roleText.text = ship.role;

        for (int i = 0; i < dots.Length; i++)
            dots[i].color = i == viewing ? dotOn : dotOff;

        float[] values = { ship.speed, ship.handling, ship.boost };
        for (int i = 0; i < statFills.Length; i++)
        {
            RectTransform fill = statFills[i];
            float from = animateStats ? fill.anchorMax.x : 0f;
            float to = values[i];
            Tween.Value(fill, "fill", 0.5f, Ease.OutCubic, k => fill.anchorMax = new Vector2(Mathf.Lerp(from, to, k), 1f), null, 0.1f + i * 0.08f);
        }
        RefreshAction();
    }

    void RefreshAction()
    {
        bool owned = PlayerData.OwnsShip(viewing);
        bool equipped = PlayerData.EquippedShip == viewing;
        if (equipped)
        {
            action.SetInteractable(false);
            action.SetLabel("EQUIPPED");
            action.SetIcon(tickSprite);
        }
        else if (owned)
        {
            action.SetInteractable(true);
            action.SetLabel("EQUIP");
            action.SetIcon(tickSprite);
        }
        else
        {
            action.SetInteractable(true);
            action.SetLabel("UNLOCK  " + GameCatalog.Ships[viewing].price.ToString("N0"));
            action.SetIcon(lockSprite);
        }
    }

    void OnAction()
    {
        if (PlayerData.OwnsShip(viewing))
        {
            PlayerData.Equip(viewing);
            UiFeedback.Success();
            UIAnim.Punch(shipRect, 1.15f);
            RefreshAction();
        }
        else
        {
            GameSession.loadoutView = viewing;
            ScreenManager.Instance.Open(ScreenId.Shop);
        }
    }
}
