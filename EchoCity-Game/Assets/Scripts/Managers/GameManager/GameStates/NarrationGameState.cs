using UnityEngine;
using System;

namespace EchoCity
{
    public class NarrationGameState : GameState
    {
        public NarrationGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }

        public override GameStatesEnum GetEnum() => GameStatesEnum.Narration;
        public override void Enter()
        {
            _gameManager.DisablePlayerInputEvent.RaiseEvent();
            _gameManager.EnableUIInputEvent.RaiseEvent();
            Time.timeScale = 0;
        }
        public void EnterNarration(DialogData data)
        {
            Enter();
            _gameManager.DialogDataEvent.RaiseEvent(data);
        }
        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToWinHandler() => _fsm.SwitchState(_fsm.WinState);
        public override void Update() { }
        public override void Exit() { }

    }
}
