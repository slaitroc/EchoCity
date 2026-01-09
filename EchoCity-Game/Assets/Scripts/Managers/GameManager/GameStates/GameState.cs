namespace EchoCity
{
    public abstract class GameState : IGameState
    {
        protected IGMContext _context;
        protected GameManagerFSM _fsm;

        public GameState(IGMContext context, GameManagerFSM fsm)
        {
            _context = context;
            _fsm = fsm;
        }

        public abstract GameStatesEnum GetEnum();
        public virtual void Enter() { }
        public virtual void EnterLoading() { }
        public virtual void Update() { }
        public virtual void Exit() { }
        public virtual void ExitLoading() { }
        public virtual void SwitchToTitleHandler(GameStatesEnum previousState) { }
        public virtual void SwitchToPlayingHandler(GameStatesEnum previousState) { }
        public virtual void SwitchToPauseHandler(GameStatesEnum previousState) { }
        public virtual void SwitchToDeathHandler(GameStatesEnum previousState) { }
        public virtual void SwitchToWinHandler(GameStatesEnum previousState) { }
        public virtual void SwitchToNarrationHandler(GameStatesEnum previousState, ToNarrationParams @params) { }
        public virtual void SwitchToHudHandler(GameStatesEnum previousState, HudEnum hud) { }
        public virtual void InitLevelHandler(SceneEnum scene)
        {
            _context.LevelActionEvent.RaiseEvent(_context, LevelActionCodeEnum.LoadActiveLevel, scene);
        }
    }
}