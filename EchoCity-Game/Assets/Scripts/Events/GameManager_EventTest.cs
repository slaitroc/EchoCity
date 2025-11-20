using System;
using UnityEngine;

public class GameManager_EventTest : MonoBehaviour
{
    [SerializeField] private GameStatesFSM gameStateFSM;
    [SerializeField] private bool pause;


    void OnEnable()
    {
        gameStateFSM = new GameStatesFSM();
        gameStateFSM.Initialize();

        pause = false;
    }

    public void PauseGameHandler()
    {
        if (!pause) gameStateFSM.ChangeState(GameStatesEnum.PAUSE);
        else gameStateFSM.ChangeState(GameStatesEnum.PLAYING);
        pause = !pause;
    }

    public void WriteMessageHandler()
    {
        Log.D("Void event triggered by pressing 'E'", "green", "GAME MANAGER"); // NOTE: 'E' is hardcoded
    }

    public void WriteMessageHandler(string message)
    {
        Log.D(message, "green", "GAME MANAGER");
    }

    public void WriteMessageHandler(int value)
    {
        Log.D("Int event raised with value: " + value, "green", "GAME MANAGER");
    }
}
