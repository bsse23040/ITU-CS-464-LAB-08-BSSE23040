using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Tells UIAudio what happened to a button: pointer over it or selected (hover), pressed (click), or pressed
// while it is disabled (error). It has no AudioSource of its own.
// The back arrows are silent here: going back has its own sound, played when the screen changes.
[RequireComponent(typeof(Button))]
public class UiSound : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerDownHandler
{
    [SerializeField] bool silentPress;

    Button button;

    void Awake() { button = GetComponent<Button>(); }

    void OnEnable() { button.onClick.AddListener(OnClick); }
    void OnDisable() { button.onClick.RemoveListener(OnClick); }

    public void OnPointerEnter(PointerEventData e) { HoverIfUsable(); }
    public void OnSelect(BaseEventData e) { HoverIfUsable(); }

    // a disabled button gets no onClick, so the "no" is heard here
    public void OnPointerDown(PointerEventData e)
    {
        if (!button.interactable && UIAudio.Instance != null) UIAudio.Instance.PlayError();
    }

    void HoverIfUsable()
    {
        if (button.interactable && UIAudio.Instance != null) UIAudio.Instance.Hover();
    }

    void OnClick()
    {
        if (!silentPress && UIAudio.Instance != null) UIAudio.Instance.Click();
    }
}
