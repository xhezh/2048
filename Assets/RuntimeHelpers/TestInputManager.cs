#if UNITY_EDITOR
using UnityEngine;

public class TestInputManager : InputManager
{
    public bool keyboardHandled = false;
    public bool mouseHandled = false;
    public bool touchHandled = false;

    protected override void HandleKeyboardInput()
    {
        keyboardHandled = true;
    }

    protected override void HandleMouseSwipe()
    {
        mouseHandled = true;
    }

    protected override void HandleTouchSwipe()
    {
        touchHandled = true;
    }
}
#endif
