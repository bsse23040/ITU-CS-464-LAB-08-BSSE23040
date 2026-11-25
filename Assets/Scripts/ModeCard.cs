using TMPro;
using UnityEngine;
using UnityEngine.UI;

// One card on the mode screen. It can be normal, picked (amber border and a tick) or locked (dim, with a lock).
public class ModeCard : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image background;
    [SerializeField] Image border;
    [SerializeField] Image tick;
    [SerializeField] Image lockIcon;
    [SerializeField] Image icon;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text blurb;
    [SerializeField] Color normalBackground = Color.gray;
    [SerializeField] Color lockedBackground = Color.black;
    [SerializeField] Color normalText = Color.white;
    [SerializeField] Color dimText = Color.gray;

    public Button Button => button;
    public bool Locked { get; private set; }

    public void Setup(GameCatalog.Mode mode, bool locked)
    {
        Locked = locked;
        title.text = mode.name;
        blurb.text = locked ? "Reach sector " + mode.unlockLevel + " to unlock." : mode.blurb;
        background.color = locked ? lockedBackground : normalBackground;
        title.color = locked ? dimText : normalText;
        blurb.color = dimText;
        icon.color = locked ? dimText : Color.white;
        lockIcon.gameObject.SetActive(locked);
        SetSelected(false, false);
    }

    public void SetSelected(bool selected, bool animate)
    {
        var borderColor = border.color;
        borderColor.a = selected ? 1f : 0f;
        if (animate)
        {
            Tween.Tint(border, borderColor, 0.2f);
            Tween.Scale(transform, selected ? 1.02f : 1f, 0.3f, Ease.OutBack);
            tick.transform.localScale = Vector3.zero;
            if (selected) Tween.Scale(tick.transform, 1f, 0.35f, Ease.OutBack, 0.05f);
        }
        else
        {
            border.color = borderColor;
            transform.localScale = Vector3.one * (selected ? 1.02f : 1f);
            tick.transform.localScale = selected ? Vector3.one : Vector3.zero;
        }
    }
}
