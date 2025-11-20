using UnityEngine;
using System;

public class GameStateNarration : IGameState
{
    public void Enter()
    {
    }

    public void Update()
    {
    }
    public void Exit()
    {
    }

    public GameStatesEnum GetEnum() => GameStatesEnum.NARRATION;
}