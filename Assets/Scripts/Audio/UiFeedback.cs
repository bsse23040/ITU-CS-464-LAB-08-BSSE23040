using System;

// "That worked" and "that did not work", said by the screens and heard by whoever wants to react.
// The shop, the level map and the settings raise these. They never touch a sound themselves.
// The events are static, so every listener must remove itself again in OnDisable.
public static class UiFeedback
{
    public static event Action Succeeded;
    public static event Action Failed;

    public static void Success() { Succeeded?.Invoke(); }
    public static void Error() { Failed?.Invoke(); }
}
