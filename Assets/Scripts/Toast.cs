using TMPro;
using UnityEngine;

// A small message that drops in from the top, waits, and leaves.
// It tells the player why something did not work, for example "Not enough coins".
public class Toast : MonoBehaviour
{
    public static Toast Instance { get; private set; }

    [SerializeField] RectTransform box;
    [SerializeField] TMP_Text text;
    [SerializeField] float shownY = -150f;
    [SerializeField] float hiddenY = 260f;

    int ticket;

    void Awake()
    {
        Instance = this;
        box.anchoredPosition = new Vector2(0f, hiddenY);
    }

    public static void Show(string message)
    {
        if (Instance != null) Instance.Display(message);
    }

    void Display(string message)
    {
        text.text = message;
        int mine = ++ticket;
        Tween.Move(box, new Vector2(0f, shownY), 0.4f, Ease.OutBack);
        Tween.Value(box, "hold", 1.9f, Ease.Linear, k => { }, () =>
        {
            if (mine == ticket) Tween.Move(box, new Vector2(0f, hiddenY), 0.3f, Ease.InCubic);
        });
    }
}
