using UnityEngine;

namespace EchoCity
{
    public class NarrationGameState : GameState
    {
        private ToNarrationParams _params;
        private bool _useCached;
        public NarrationGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }

        public override GameStatesEnum GetEnum() => GameStatesEnum.Narration;
        public override void Enter()
        {
            Time.timeScale = 1;
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.Narration, new NarrationParams(_params, _useCached));
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, true);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
        }
        public void EnterNarration(ToNarrationParams toNarrationParams)
        {
            _useCached = false;
            _params = toNarrationParams;
            Enter();
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            if (_params.DestinationScene != SceneEnum.None)
                _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, _params.DestinationScene);
            else
                _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.UnloadLevel, SceneEnum.None);
        }
        public override void ExitLoading()
        {
            GameStatesEnum nextState = _params.NextGameState;
            if (nextState == GameStatesEnum.Playing)
                _fsm.SwitchState(_fsm.PlayingState);
            else if (nextState == GameStatesEnum.Title)
                _fsm.SwitchState(_fsm.TitleState);
            else if (nextState == GameStatesEnum.Win)
                _fsm.SwitchState(_fsm.WinState);
            else if (nextState == GameStatesEnum.Death)
                _fsm.SwitchState(_fsm.DeathState);
        }
        public override void SwitchToPauseHandler(GameStatesEnum previousState)
        {
            _useCached = true;
            _fsm.SwitchState(_fsm.PauseState);
        }
        public override void Update() { }
        public override void Exit() { }

    }
}
