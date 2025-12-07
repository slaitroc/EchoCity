using System;
using UnityEngine;
using UnityEngine.InputSystem;


namespace EchoCity
{
    public class PauseGameState : GameState
    {
        private bool _toTitle = false;
        private bool _restart = false;
        public PauseGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }
        public override void Enter()
        {
            Time.timeScale = 0;
            _gameManager.DisablePlayerInputEvent.RaiseEvent();
            _gameManager.EnableUIInputEvent.RaiseEvent();
            _gameManager.PauseMenuEvent.RaiseEvent();
        }
        public override void Update(){}
        public override void Exit()
        {
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
        public override GameStatesEnum GetEnum() => GameStatesEnum.Pause;

        public override void SwitchToPlayingHandler() => _fsm.SwitchState(_fsm.PlayingState);
        public override void SwitchToTitleHandler()
        {
            _toTitle = true;
            _gameManager.UnloadCurrentLevelEvent.RaiseEvent();
        }
        public override void SwitchToInitLevelHandler(SceneEnum scene)
        {
            _restart = true;
            _gameManager.ReloadLevelEvent.RaiseEvent();
        }
    }
}