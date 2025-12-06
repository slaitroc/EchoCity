using UnityEngine;

namespace EchoCity
{
    public class TitleGameState : GameState
    {
        public TitleGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm)
        {
        }

        public override void Enter()
        {//TODO audio
            Time.timeScale = 0;
            _gameManager.DisablePlayerInputEvent.RaiseEvent();
            _gameManager.EnableUIInputEvent.RaiseEvent();
            _gameManager.TitleMenuEvent.RaiseEvent();
        }
        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        public override GameStatesEnum GetEnum()
        {
            return GameStatesEnum.Hud;
        }

        public override void SwitchToInitLevelHandler(SceneEnum scene)
        {
            _gameManager.LoadLevelEvent.RaiseEvent(scene);
            // _fsm.SwitchState(_fsm.InitLevelState);
        }

        public override void EnterLoading() { }

        public override void ExitLoading()
        {
            _gameManager.DisablePlayerInputEvent.RaiseEvent();
            _gameManager.EnableUIInputEvent.RaiseEvent();
            _fsm.SwitchState(_fsm.PlayingState);
        }
    }
}