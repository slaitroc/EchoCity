using UnityEngine;

namespace EchoCity
{
    public class WinGameState : GameState
    {
        private bool _toTitle = false;
        private bool _restart = false;

        public WinGameState(GameManager gameManager, GameStatesFSM fsm) : base(gameManager, fsm) { }
        public override void Enter()
        {
            Time.timeScale = 0;
            _gameManager.DisablePlayerInputEvent.RaiseEvent();
            _gameManager.EnableUIInputEvent.RaiseEvent();
            _gameManager.WinMenuEvent.RaiseEvent();
        }
        public override void Update() { }
        public override void Exit()
        {
            if (_restart)
                _gameManager.SetPlayerOnSpawnEvent.RaiseEvent();
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
        public override GameStatesEnum GetEnum() => GameStatesEnum.Win;
        public override void SwitchToTitleHandler()
        {
            _toTitle = true;
            _gameManager.UnloadCurrentLevelEvent.RaiseEvent();
        }
        public override void SwitchToInitLevelHandler(SceneEnum scene)
        {
            _restart = true;
            _gameManager.LoadLevelEvent.RaiseEvent(scene);
        }
    }
}