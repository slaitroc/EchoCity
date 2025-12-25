using UnityEngine;
using System;

namespace EchoCity
{
    public class PlayingGameState : GameState
    {
        public PlayingGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override void Enter()
        {
            Time.timeScale = 1;
            _gameManager.DisableUIInputEvent.RaiseEvent();
            _gameManager.EnablePlayerInputEvent.RaiseEvent();
        }
        public override void Update() { }
        public override void Exit() { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Playing;
        public override void SwitchToPauseHandler() => _fsm.SwitchState(_fsm.PauseState);
        public override void SwitchToDeathHandler() => _fsm.SwitchState(_fsm.DeathState);
        public override void SwitchToWinHandler() => _fsm.SwitchState(_fsm.WinState);
        public override void SwitchToNarrationHandler(DialogData data) => _fsm.SwitchToNarration(data);
        public override void SwitchToHudHandler(HudEnum hud) => _fsm.SwitchToHud(hud);

        public override void EnterLoading() { }
        public override void ExitLoading() { }
    }
}
