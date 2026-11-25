using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Keeps the look of one button in one place: its colours when it works and when it does not.
// A disabled button has a dimmer fill, dimmer text and usually a lock or a tick,
// so it never depends on colour alone.
[RequireComponent(typeof(Button))]
public class StyledButton : MonoBehaviour
{
    public Image background;
    public TMP_Text label;
    public Image icon;
    public Color enabledBackground = Color.white;
    public Color enabledText = Color.black;
    public Color disabledBackground = Color.gray;
    public Color disabledText = Color.white;

    Button button;

    public Button Button
    {
        get
        {
            if (button == null) button = GetComponent<Button>();
            return button;
        }
    }

    public void SetInteractable(bool on)
    {
        Button.interactable = on;
        if (background != null) background.color = on ? enabledBackground : disabledBackground;
        if (label != null) label.color = on ? enabledText : disabledText;
        if (icon != null) icon.color = on ? enabledText : disabledText;
    }

    public void SetLabel(string text)
    {
        if (label != null) label.text = text;
    }

    public void SetIcon(Sprite sprite)
    {
        if (icon != null) icon.sprite = sprite;
    }
}
