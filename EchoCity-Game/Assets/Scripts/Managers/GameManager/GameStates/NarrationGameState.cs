using UnityEngine;
using System;

namespace EchoCity
{
    public class NarrationGameState : GameState
    {
        public NarrationGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }

        public override GameStatesEnum GetEnum() => GameStatesEnum.Narration;
        public override void Enter()
        {
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
            Time.timeScale = 0;
        }
        public void EnterNarration(DialogData data)
        {
            Enter();
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.Dialog, new DialogParams(data));
        }
        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToWinHandler() => _fsm.SwitchState(_fsm.WinState);
        public override void Update() { }
        public override void Exit() { }

    }
}
