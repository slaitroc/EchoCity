using UnityEngine;
using System;

public class GameStatePlaying : IGameState
{
    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    public GameStatesEnum GetEnum() => GameStatesEnum.PLAYING;

}