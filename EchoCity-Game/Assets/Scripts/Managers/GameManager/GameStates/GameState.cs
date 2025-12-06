namespace EchoCity
{
    public abstract class GameState
    {
        #region Constants
        protected string _LOG_TAG = "GAME STATE";
        protected string _LOG_COLOR = "#00ff00ff";
        #endregion


        protected GameManager _gameManager;
        protected GameStatesFSM _fsm;

        public GameState(GameManager gameManager, GameStatesFSM fsm)
        {
            this._gameManager = gameManager;
            this._fsm = fsm;
        }

        public abstract GameStatesEnum GetEnum();
        public virtual void Enter() { }
        public virtual void EnterLoading() { }
        public virtual void Update() { }
        public virtual void Exit() { }
        public virtual void ExitLoading() { }
        public virtual void SwitchToTitleHandler() { }
        public virtual void SwitchToInitLevelHandler(SceneEnum scene) { }
        public virtual void SwitchToPlayingHandler() { }
        public virtual void SwitchToPauseHandler() { }
        public virtual void SwitchToDeathHandler() { }
        public virtual void SwitchToNarrationHandler(DialogData data) { }
        public virtual void SwitchToHudHandler(HUDEnum hud) { }
    }
}