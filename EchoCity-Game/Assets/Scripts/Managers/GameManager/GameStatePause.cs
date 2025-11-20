using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class GameStatePause : IGameState
{
    [Header("UI Action Map")]
    private const string UI_ACTION_MAP = "UI";
    private PlayerInput _playerInput;
    private string _previousActionMap;

    public void Enter()
    {
        Time.timeScale = 0;
        
        _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        SwitchToUIActionMap();
    }
    public void Update()
    {
    }

    public void Exit()
    {
        Time.timeScale = 1;
        RestorePreviousActionMap();
    }

    public GameStatesEnum GetEnum() => GameStatesEnum.PAUSE;
    
    
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
    

}