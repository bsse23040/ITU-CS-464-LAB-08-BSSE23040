using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Moves the ship toward the finger (or the held mouse button) and makes it blink after a hit.
public class ShipPilot : MonoBehaviour
{
    [SerializeField] RectTransform area;
    [SerializeField] RectTransform ship;
    [SerializeField] Image shipImage;
    [SerializeField] Image shieldRing;
    [SerializeField] ShipHealth health;
    [SerializeField] Sprite[] shipSprites;

    float targetX;
    bool pressedOnButton;      // a press that started on Pause or Boost must not steer the ship

    public void ResetShip()
    {
        shipImage.sprite = shipSprites[PlayerData.EquippedShip];
        targetX = 0f;
        ship.anchoredPosition = new Vector2(0f, ship.anchoredPosition.y);
        shipImage.color = Color.white;
    }

    public void Tick(float dt)
    {
        if (Input.GetMouseButtonDown(0))
            pressedOnButton = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButton(0) && !pressedOnButton)
        {
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(area, Input.mousePosition, null, out local);
            targetX = local.x;
        }
        float half = area.rect.width / 2f - 90f;
        targetX = Mathf.Clamp(targetX, -half, half);

        // the Steering setting decides how quickly the ship follows the finger
        float follow = Mathf.Lerp(5f, 18f, PlayerData.Steering);
        Vector2 p = ship.anchoredPosition;
        float x = Mathf.Lerp(p.x, targetX, 1f - Mathf.Exp(-follow * dt));
        ship.anchoredPosition = new Vector2(x, p.y);
        ship.localRotation = Quaternion.Euler(0f, 0f, Mathf.Clamp((p.x - targetX) * 0.06f, -14f, 14f));

        bool hidden = health.Hurt && Mathf.FloorToInt(Time.time * 12f) % 2 == 0;
        shipImage.color = new Color(1f, 1f, 1f, hidden ? 0.25f : 1f);
        shieldRing.enabled = health.Shielded;
    }
}
