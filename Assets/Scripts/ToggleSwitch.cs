using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// An on/off switch. The knob slides across and the track changes colour,
// and the word ON or OFF is written inside it too, so the state is not only a colour.
[RequireComponent(typeof(Button))]
public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] Image track;
    [SerializeField] RectTransform knob;
    [SerializeField] TMP_Text stateText;
    [SerializeField] Color onColor = Color.green;
    [SerializeField] Color offColor = Color.gray;
    [SerializeField] Color onTextColor = Color.black;     // dark on the bright ON track
    [SerializeField] Color offTextColor = Color.white;    // light on the dim OFF track
    [SerializeField] float travel = 54f;

    public event Action<bool> Toggled;
    public bool Value { get; private set; }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            Set(!Value, true);
            if (Toggled != null) Toggled(Value);
        });
    }

    public void Set(bool on, bool animate)
    {
        Value = on;
        Vector2 knobTarget = new Vector2(on ? travel : -travel, 0f);
        Color trackTarget = on ? onColor : offColor;

        if (animate)
        {
            Tween.Move(knob, knobTarget, 0.22f, Ease.InOutCubic);
            Tween.Tint(track, trackTarget, 0.22f, Ease.InOutCubic);
        }
        else
        {
            knob.anchoredPosition = knobTarget;
            track.color = trackTarget;
        }

        stateText.text = on ? "ON" : "OFF";
        stateText.color = on ? onTextColor : offTextColor;
        stateText.alignment = on ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
    }
}
