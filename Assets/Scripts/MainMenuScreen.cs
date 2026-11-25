using TMPro;
using UnityEngine;
using UnityEngine.UI;

// The hub. It shows the ship you have equipped and the five places you can go.
// Everything pops in one after another from top to bottom, so the eye reads it in that order.
public class MainMenuScreen : UIScreen
{
    [SerializeField] RectTransform[] popItems;
    [SerializeField] RectTransform hero;
    [SerializeField] Image shipImage;
    [SerializeField] TMP_Text shipName;
    [SerializeField] TMP_Text pilotLevel;
    [SerializeField] Sprite[] shipSprites;

    Vector2 heroHome;

    protected override void Awake()
    {
        base.Awake();
        heroHome = hero.anchoredPosition;
    }

    public override void OnOpen()
    {
        int ship = PlayerData.EquippedShip;
        shipImage.sprite = shipSprites[ship];
        shipName.text = GameCatalog.Ships[ship].name;
        pilotLevel.text = PlayerData.PilotLevel.ToString();

        for (int i = 0; i < popItems.Length; i++)
            UIAnim.PopIn(popItems[i], 0.08f + i * 0.07f);

        Tween.Loop(hero, "float", 1.8f, Ease.InOutSine,
            k => hero.anchoredPosition = heroHome + new Vector2(0f, Mathf.Lerp(-16f, 16f, k)));
    }

    public override void OnClose()
    {
        Tween.Cancel(hero, "float");
    }
}
