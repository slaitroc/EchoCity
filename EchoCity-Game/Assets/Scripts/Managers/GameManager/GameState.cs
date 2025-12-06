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

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
        public abstract GameStatesEnum GetEnum();

        public abstract bool PauseGameHandler();
        public abstract bool DeathHandler();
    }
}