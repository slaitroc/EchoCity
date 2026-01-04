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

        public override void ExitLoading()
        {
            if (_toTitle)
            {
                _fsm.SwitchState(_fsm.TitleState);
                _toTitle = false;
            }
            if (_loadLevel)
            {
                _context.SetPlayerOnSpawnEvent.RaiseEvent(_context);
                _fsm.SwitchState(_fsm.PlayingState);
                _loadLevel = false;
            }
        }
        public override GameStatesEnum GetEnum() => GameStatesEnum.Pause;

        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToTitleHandler()
        {
            _toTitle = true;
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.UnloadLevel);
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            _loadLevel = true;
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, scene);
        }
    }
}