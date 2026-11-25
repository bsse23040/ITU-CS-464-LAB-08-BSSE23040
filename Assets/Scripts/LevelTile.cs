using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One square on the sector map. Three looks:
//   locked   dark with a lock and no number
//   open     a number and three stars, full for the ones already earned and an outline for the rest
//   picked   the same, with a thick amber border
public class LevelTile : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image background;
    [SerializeField] Image border;
    [SerializeField] Image lockIcon;
    [SerializeField] TMP_Text number;
    [SerializeField] Image[] stars;
    [SerializeField] Sprite starFull;
    [SerializeField] Sprite starEmpty;
    [SerializeField] Color openBackground = Color.blue;
    [SerializeField] Color lockedBackground = Color.black;
    [SerializeField] Color starOn = Color.yellow;
    [SerializeField] Color starOff = Color.gray;

    public Button Button => button;
    public bool Locked { get; private set; }

    public void Refresh(int index, bool selected)
    {
        Locked = !PlayerData.IsUnlocked(index);
        number.text = (index + 1).ToString();
        number.gameObject.SetActive(!Locked);
        lockIcon.gameObject.SetActive(Locked);
        background.color = Locked ? lockedBackground : openBackground;

        int earned = PlayerData.Stars(index);
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].gameObject.SetActive(!Locked);
            bool got = i < earned;
            stars[i].sprite = got ? starFull : starEmpty;
            stars[i].color = got ? starOn : starOff;
        }

        var c = border.color;
        c.a = selected ? 1f : 0f;
        border.color = c;
    }

    public void SetSelected(bool selected)
    {
        var c = border.color;
        c.a = selected ? 1f : 0f;
        Tween.Tint(border, c, 0.15f);
        if (selected) UIAnim.Punch(transform, 1.08f);
    }
}
