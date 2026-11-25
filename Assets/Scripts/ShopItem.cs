using TMPro;
using UnityEngine;

// One card in the shop, for a ship or for a boost.
public class ShopItem : MonoBehaviour
{
    [SerializeField] bool isShip = true;
    [SerializeField] int index;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text subText;
    [SerializeField] StyledButton buy;
    [SerializeField] Sprite coinSprite;
    [SerializeField] Sprite tickSprite;

    public bool IsShip => isShip;
    public int Index => index;
    public StyledButton Buy => buy;

    public void Refresh()
    {
        if (isShip)
        {
            var ship = GameCatalog.Ships[index];
            nameText.text = ship.name;
            subText.text = ship.role;
            if (PlayerData.OwnsShip(index))
            {
                buy.SetInteractable(false);
                buy.SetLabel("OWNED");
                buy.SetIcon(tickSprite);
            }
            else
            {
                buy.SetInteractable(true);
                buy.SetLabel(ship.price.ToString("N0"));
                buy.SetIcon(coinSprite);
            }
        }
        else
        {
            var boost = GameCatalog.Boosts[index];
            nameText.text = boost.name;
            subText.text = boost.blurb + "  (you have " + PlayerData.BoostCount(index) + ")";
            buy.SetInteractable(true);
            buy.SetLabel(boost.price.ToString("N0"));
            buy.SetIcon(coinSprite);
        }
    }
}
