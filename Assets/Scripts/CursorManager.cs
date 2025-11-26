using UnityEngine;

public static class CursorManager
{
    public static void HideAndLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public static void Hide()
    {
        Cursor.visible = false;
    }

    public static void ShowAndUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

