using UnityEngine;

namespace EchoCity
{
    public class TitleGameState : GameState
    {
        public TitleGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }
        public override void Enter()
        {//TODO audio
            Time.timeScale = 0;
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.TitleMenu, null);
        }
        public override void Update() { }
        public override void Exit() { }

        public override GameStatesEnum GetEnum() => GameStatesEnum.Title;
        public override void InitLevelHandler(SceneEnum scene) => _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, scene);
        public override void EnterLoading() { }
        public override void ExitLoading()
        {
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.Player, false);
            _context.PlayerInputEvent.RaiseEvent(_context, InputEnum.UI, true);
            _fsm.SwitchState(_fsm.PlayingState);
        }
    }
}