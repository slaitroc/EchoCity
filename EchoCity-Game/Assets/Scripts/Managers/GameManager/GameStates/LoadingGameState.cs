using UnityEngine;

namespace EchoCity
{
    public class LoadingGameState : GameState
    {
        public LoadingGameState(IGMContext context, GameManagerFSM fsm) : base(context, fsm) { }

        public override void Enter()
        {
            _gameManager.EnterLoadingScreenEvent.RaiseEvent();
            _gameManager.DisablePlayerInputEvent.RaiseEvent();
            _gameManager.DisableUIInputEvent.RaiseEvent();
        }
        public override void Update() { }
        public override void Exit() => _gameManager.ExitLoadingScreenEvent.RaiseEvent();

        public override GameStatesEnum GetEnum() => GameStatesEnum.Loading;
    }
}