using TMPro;
using UnityEngine;

// The coin and gem counters. When an amount changes the number counts up or down to the new value,
// so a purchase or a reward is noticed instead of the text just flipping.
public class CurrencyBar : MonoBehaviour
{
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text gemText;

    int shownCoins;
    int shownGems;

    void OnEnable()
    {
        PlayerData.Changed += Refresh;
        // jump straight to the real numbers when the screen opens
        shownCoins = PlayerData.Coins;
        shownGems = PlayerData.Gems;
        coinText.text = shownCoins.ToString("N0");
        gemText.text = shownGems.ToString("N0");
    }

    void OnDisable()
    {
        PlayerData.Changed -= Refresh;
    }

    void Refresh()
    {
        if (shownCoins != PlayerData.Coins)
        {
            Tween.Count(this, "coins", shownCoins, PlayerData.Coins, 0.6f, Ease.OutQuad, v => coinText.text = v.ToString("N0"));
            shownCoins = PlayerData.Coins;
        }
        if (shownGems != PlayerData.Gems)
        {
            Tween.Count(this, "gems", shownGems, PlayerData.Gems, 0.6f, Ease.OutQuad, v => gemText.text = v.ToString("N0"));
            shownGems = PlayerData.Gems;
        }
    }
}
