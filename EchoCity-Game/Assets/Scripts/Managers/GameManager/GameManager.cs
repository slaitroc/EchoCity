using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
#pragma warning disable CS0414
    #region Constants
    private string _LOG_TAG = "GAME MANAGER";
    private string _LOG_COLOR = "#00ff00ff";
    #endregion
#pragma warning restore CS0414

    #region Serialized Fields
    [Header("Invoking Events")]
    [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;

    #endregion

    #region Private Fields
    private GameStatesFSM _fsm;
    public GameStatesEnum CurrentState;
    #endregion

    void Awake()
    {
        _fsm = new GameStatesFSM(this);
        _fsm.Initialize();
    }

    void Update()
    {
        _fsm.Update();
    }

    public void RaiseSwitchStateEvent(GameStatesEnum from, GameStatesEnum to)
    {
        switchGameStateEvent.RaiseEvent(from, to);
        Log.D($"Switch Game State Event Raised from {from} to {to}", _LOG_COLOR, _LOG_TAG);
    }

    public void PauseGameHandler()
    {
        _fsm.CurrentState.PauseGameHandler();
    }

}