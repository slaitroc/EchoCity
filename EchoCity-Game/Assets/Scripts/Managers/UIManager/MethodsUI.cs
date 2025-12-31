using UnityEngine;

public static class MethodsUI
{

    public static void SetCursorState(bool visible = true)
    {
        if (visible)
            ShowCursor();
        else
            HideCursor();
    }
    public static void ShowCursor()
    {
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    public static void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

}
