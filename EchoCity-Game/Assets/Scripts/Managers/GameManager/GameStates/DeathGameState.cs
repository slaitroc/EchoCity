using UnityEngine;


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
        public override void Exit() { }
        public override void ExitLoading()
        {
            if (_toTitle)
            {
                _fsm.SwitchState(_fsm.TitleState);
                _toTitle = false;
            }
            if (_restart)
            {
                _context.SetPlayerOnSpawnEvent.RaiseEvent(_context);
                _fsm.SwitchState(_fsm.PlayingState);
                _restart = false;
            }
        }
        public override void SwitchToTitleHandler(GameStatesEnum fromState)
        {
            _toTitle = true;
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.UnloadLevel);
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            _restart = true;
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, scene);
        }
    }
}
