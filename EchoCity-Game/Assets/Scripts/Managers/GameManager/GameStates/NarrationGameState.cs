using UnityEngine;

namespace EchoCity
{
    public class NarrationGameState : GameState
    {
        private SODialogContainer _container;
        public NarrationGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }

        public override GameStatesEnum GetEnum() => GameStatesEnum.Narration;
        public override void Enter()
        {
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
            Time.timeScale = 0;
        }
        public void EnterNarration(ToNarrationParams toNarrationParams)
        {
            Enter();
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.Narration, new NarrationParams(toNarrationParams));
        }
        public override void InitLevelHandler(SceneEnum scene)
        {
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, scene);
        }
        public override void ExitLoading()
        {
            _fsm.SwitchState(_fsm.PlayingState);
        }
        public override void SwitchToPauseHandler(GameStatesEnum previousState) => _fsm.SwitchState(_fsm.PauseState);
        public override void Update() { }
        public override void Exit() { }

    }
}
