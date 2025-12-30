using UnityEngine;

namespace EchoCity
{
    public class LoadingGameState : GameState
    {
        public LoadingGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }

        public override void Enter()
        {
            _context.EnterLoadingScreenEvent.RaiseEvent(_context);
            _context.DisablePlayerInputEvent.RaiseEvent(_context);
            _context.DisableUIInputEvent.RaiseEvent(_context);
        }
        public override void Update() { }
        public override void Exit() => _context.ExitLoadingScreenEvent.RaiseEvent(_context);

        public override GameStatesEnum GetEnum() => GameStatesEnum.Loading;
    }
}