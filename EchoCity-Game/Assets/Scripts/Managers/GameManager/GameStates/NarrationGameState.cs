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
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, scene);
        }
        public override void ExitLoading()
        {
            _fsm.SwitchState(_fsm.PlayingState);
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
