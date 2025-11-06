using System;
using UnityEngine;

public class GameManager_EventTest : MonoBehaviour
{
    public void WriteMessage()
    {
        Log.D("Void event triggered by pressing 'E'", "green", "GAME MANAGER"); // NOTE: 'E' is hardcoded
    }

    public void WriteMessage(string message)
    {
        Log.D(message, "green", "GAME MANAGER");
    }

    public void WriteIntMessage(int value)
    {
        Log.D("Int event raised with value: " + value, "green", "GAME MANAGER");
    }
}
