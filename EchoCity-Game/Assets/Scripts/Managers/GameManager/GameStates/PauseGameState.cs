using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace EchoCity
{
    public class PauseGameState : GameState
    {
        private bool _toTitle = false;
        private bool _loadLevel = false;
        public PauseGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override void Enter()
        {
            Time.timeScale = 0;
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.PauseMenu, null);
        }
        public override void Update() { }
        public override void Exit()
        {
            _toTitle = false;
            _loadLevel = false;
        }
        public override void ExitLoading()
        {
            if (_toTitle)
                _fsm.SwitchState(_fsm.TitleState);
            if (_loadLevel)
                _fsm.SwitchState(_fsm.PlayingState);
        }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Pause;

        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToTitleHandler()
        {
            _toTitle = true;
            _context.UnloadCurrentSceneEvent.RaiseEvent(_context);
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            _loadLevel = true;
            _context.LoadSceneEvent.RaiseEvent(_context, scene);
        }
    }
}