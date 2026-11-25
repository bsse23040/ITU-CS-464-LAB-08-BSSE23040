using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Gives a button a physical feel. It shrinks and goes a bit darker while it is held,
// then springs back with a small overshoot when the finger lifts.
[RequireComponent(typeof(Button))]
public class PressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] float pressedScale = 0.94f;
    [SerializeField] float darken = 0.82f;

    Button button;
    Graphic graphic;
    Color before;
    bool pressed;

    void Awake()
    {
        button = GetComponent<Button>();
        graphic = GetComponent<Graphic>();
    }

    void OnDisable()
    {
        // a screen can hide while a finger is still down, so put everything back
        if (pressed) Release(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        pressed = true;
        Tween.Scale(transform, pressedScale, 0.08f, Ease.OutQuad);
        if (graphic != null)
        {
            before = graphic.color;
            graphic.color = new Color(before.r * darken, before.g * darken, before.b * darken, before.a);
        }
    }

    public void OnPointerUp(PointerEventData eventData) { Release(true); }
    public void OnPointerExit(PointerEventData eventData) { Release(true); }

    void Release(bool animate)
    {
        if (!pressed) return;
        pressed = false;
        if (graphic != null) graphic.color = before;
        if (animate) Tween.Scale(transform, 1f, 0.25f, Ease.OutBack);
        else transform.localScale = Vector3.one;
    }
}
