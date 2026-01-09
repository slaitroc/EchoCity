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
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, true);
        }
        public override void Update() { }
        public override void Exit() { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Playing;
        public override void SwitchToPauseHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.PauseState);
        public override void SwitchToDeathHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.DeathState);
        public override void SwitchToWinHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.WinState);
        public override void SwitchToNarrationHandler(GameStatesEnum previousState, ToNarrationParams @params) => _fsm.SwitchToNarration(@params);
        public override void SwitchToHudHandler(GameStatesEnum previousState, HudEnum hud) => _fsm.SwitchToHud(hud);
        public override void EnterLoading() { }
        public override void ExitLoading() { }
    }
}
