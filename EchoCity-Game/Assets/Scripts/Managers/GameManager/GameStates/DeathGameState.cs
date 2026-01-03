using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace EchoCity
{
    public class DeathGameState : GameState
    {
        private bool _toTitle = false;
        private bool _restart = false;
        public DeathGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Death;
        public override void Enter()
        {
            Time.timeScale = 0;
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.DeathMenu, null);
        }
        public override void Exit()
        {
            if (_restart)
                _context.SetPlayerOnSpawnEvent.RaiseEvent(_context);
            _toTitle = false;
            _restart = false;
        }
        public override void ExitLoading()
        {
            if (_toTitle)
                _fsm.SwitchState(_fsm.TitleState);
            if (_restart)
                _fsm.SwitchState(_fsm.PlayingState);
        }
        public override void SwitchToTitleHandler()
        {
            _toTitle = true;
            _context.UnloadCurrentSceneEvent.RaiseEvent(_context);
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            _restart = true;
            _context.LoadSceneEvent.RaiseEvent(_context, scene);
        }
    }
}
