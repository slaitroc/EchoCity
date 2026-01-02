using UnityEngine;

namespace EchoCity
{
    public class WinGameState : GameState
    {
        private bool _toTitle = false;
        private bool _restart = false;

        public WinGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override void Enter()
        {
            Time.timeScale = 0;
            _context.DisablePlayerInputEvent.RaiseEvent(_context);
            _context.EnableUIInputEvent.RaiseEvent(_context);
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.WinMenu, null);
        }
        public override void Update() { }
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
        public override GameStatesEnum GetEnum() => GameStatesEnum.Win;
        public override void SwitchToTitleHandler()
        {
            _toTitle = true;
            _context.UnloadCurrentLevelEvent.RaiseEvent(_context);
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            _restart = true;
            _context.LoadLevelEvent.RaiseEvent(_context, scene);
        }
    }
}