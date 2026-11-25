using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Reports a left or right swipe on the object it sits on (it needs an Image to catch the touch).
// The ship picker uses it, so a thumb can swipe through the ships as well as tap the arrows.
public class SwipeDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] float minDistance = 90f;

    public event Action<int> Swiped;   // +1 for a swipe to the left (next), -1 for a swipe to the right (previous)

    float startX;

    public void OnBeginDrag(PointerEventData e) { startX = e.position.x; }
    public void OnDrag(PointerEventData e) { }

    public void OnEndDrag(PointerEventData e)
    {
        float dx = e.position.x - startX;
        if (Mathf.Abs(dx) < minDistance) return;
        if (Swiped != null) Swiped(dx < 0f ? 1 : -1);
    }
}
