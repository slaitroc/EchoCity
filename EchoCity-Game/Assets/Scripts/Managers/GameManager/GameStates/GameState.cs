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
        public virtual void SwitchToTitleHandler() { }
        public virtual void InitLevelHandler(SceneEnum scene) { }
        public virtual void SwitchToPlayingHandler() { }
        public virtual void SwitchToPauseHandler() { }
        public virtual void SwitchToDeathHandler() { }
        public virtual void SwitchToWinHandler() { }
        public virtual void SwitchToNarrationHandler(SODialogContainer container) { }
        public virtual void SwitchToHudHandler(HudEnum hud) { }
    }
}