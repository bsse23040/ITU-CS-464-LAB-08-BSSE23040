using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// A row of choices where exactly one is picked (Low / Medium / High, Ships / Boosts).
// The coloured pill behind the picked one slides across to the new choice.
public class SegmentedControl : MonoBehaviour
{
    [SerializeField] Button[] segments;
    [SerializeField] TMP_Text[] labels;
    [SerializeField] RectTransform highlight;
    [SerializeField] Color selectedText = Color.black;
    [SerializeField] Color normalText = Color.white;

    public event Action<int> Changed;
    public int Index { get; private set; }

    void Awake()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            int k = i;
            segments[i].onClick.AddListener(() =>
            {
                if (k == Index) return;
                Select(k, true);
                if (Changed != null) Changed(k);
            });
        }
    }

    public void Select(int index, bool animate)
    {
        Index = index;
        float x = ((RectTransform)segments[index].transform).anchoredPosition.x;
        Vector2 target = new Vector2(x, highlight.anchoredPosition.y);
        if (animate) Tween.Move(highlight, target, 0.25f, Ease.OutCubic);
        else highlight.anchoredPosition = target;

        for (int i = 0; i < labels.Length; i++)
            labels[i].color = i == index ? selectedText : normalText;
    }
}
