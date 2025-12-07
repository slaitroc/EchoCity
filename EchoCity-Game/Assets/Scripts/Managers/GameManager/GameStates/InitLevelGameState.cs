using EchoCity;
using UnityEngine;

public class InitLevelGameState : GameState
{
    public InitLevelGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }

    public override void Enter()
    {
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }

    public override GameStatesEnum GetEnum()
    {
        return GameStatesEnum.InitLevel;
    }

    public override void EnterLoading()
    {
    }

    public override void ExitLoading()
    {
    }
}