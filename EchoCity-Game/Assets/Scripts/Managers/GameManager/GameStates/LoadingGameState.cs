using UnityEngine;

namespace EchoCity
{
    public class LoadingGameState : GameState
    {
        public LoadingGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }

        public override void Enter()
        {
            _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.LoadingScreen, new LoadingParams(true));
            _context.DisablePlayerInputEvent.RaiseEvent(_context);
            _context.DisableUIInputEvent.RaiseEvent(_context);
        }
        public override void Update() { }
        public override void Exit() => _context.ShowUIEvent.RaiseEvent(_context, ShowableUIEnum.LoadingScreen, new LoadingParams(false));

        public override GameStatesEnum GetEnum() => GameStatesEnum.Loading;
    }
}