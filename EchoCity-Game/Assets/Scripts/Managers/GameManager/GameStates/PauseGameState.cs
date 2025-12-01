using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class PauseGameState : GameState
{
    [Header("UI Action Map")]
    private const string UI_ACTION_MAP = "UI";
    private PlayerInput _playerInput;
    private string _previousActionMap;

    public PauseGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm)
    {
    }

    public override void Enter()
    {
        Time.timeScale = 0;
        _playerInput = GameObject.FindGameObjectWithTag("InputManager").GetComponent<PlayerInput>();
        // SwitchToUIActionMap();
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        Time.timeScale = 1;
        // RestorePreviousActionMap();
    }

    private void SwitchToUIActionMap()
    {
        if (_playerInput != null)
        {
            _previousActionMap = _playerInput.currentActionMap.name;
            _playerInput.SwitchCurrentActionMap(UI_ACTION_MAP);
        }
    }

    private void RestorePreviousActionMap()
    {
        if (_playerInput != null) _playerInput.SwitchCurrentActionMap(_previousActionMap);
    }

    public override GameStatesEnum GetEnum()
    {
        return GameStatesEnum.Pause;
    }

    public override bool PauseGameHandler()
    {
        _fsm.SwitchState(_fsm.PreviousState);
        return true;
    }
}