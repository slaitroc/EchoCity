using System;
using UnityEngine;
using UnityEngine.InputSystem;



public class DeathState : GameState
{
    [Header("UI Action Map")]
    private const string UI_ACTION_MAP = "UI";


    public DeathState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }
    public override void Enter()
    {
        Time.timeScale = 0;
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
        Time.timeScale = 1;
    }



    public override GameStatesEnum GetEnum()
    {
        return GameStatesEnum.Death;
    }

    public override bool PauseGameHandler()
    {
        _fsm.SwitchState(_fsm.PreviousState);
        return true;
    }

    public override bool DeathHandler()
    {
        _fsm.SwitchState(_fsm.PlayingState);
        return true;
    }
}
