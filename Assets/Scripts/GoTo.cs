using UnityEngine;
using UnityEngine.UI;

// Put this on a button to make it a navigation button.
// It either opens one screen, or goes back, or opens one screen and forgets the history.
[RequireComponent(typeof(Button))]
public class GoTo : MonoBehaviour
{
    public enum Kind { Open, Back, OpenFresh }

    [SerializeField] Kind kind = Kind.Open;
    [SerializeField] ScreenId target;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Go);
    }

    void Go()
    {
        var manager = ScreenManager.Instance;
        if (manager == null) return;
        switch (kind)
        {
            case Kind.Open: manager.Open(target); break;
            case Kind.OpenFresh: manager.OpenFresh(target); break;
            case Kind.Back: manager.Back(); break;
        }
    }
}
