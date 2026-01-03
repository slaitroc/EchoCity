using System;
using System.Collections.Generic;

namespace EchoCity
{
    [Serializable]
    public class GameManagerFSM : IFSMWithLoading
    {
        private readonly IGMContext _context;
        public GameState CurrentState { get; private set; }
        public GameState PreviousState { get; private set; }
        private GameState _inLoadingState;

        public readonly GameState LoadingState;
        public readonly GameState TitleState;
        public readonly GameState PlayingState;
        public readonly GameState PauseState;
        public readonly HudGameState HudState;
        public readonly NarrationGameState NarrationState;
        public readonly GameState DeathState;
        public readonly GameState WinState;

        public GameManagerFSM(IGMContext context)
        {


            LoadingState = new LoadingGameState(context, this);
            TitleState = new TitleGameState(context, this);
            PlayingState = new PlayingGameState(context, this);
            PauseState = new PauseGameState(context, this);
            NarrationState = new NarrationGameState(context, this);
            DeathState = new DeathGameState(context, this);
            WinState = new WinGameState(context, this);
            HudState = new HudGameState(context, this);
            _context = context;
        }

        public void Initialize()
        {
            CurrentState = TitleState;
            CurrentState.Enter();
            _context.GameStateTransitionEvent.RaiseEvent(_context, GameStatesEnum.None, GameStatesEnum.Title);
        }
        public void Initialize(GameState state)
        {
            CurrentState = state;
            CurrentState.Enter();
            _context.GameStateTransitionEvent.RaiseEvent(_context, GameStatesEnum.None, state.GetEnum());
        }


        public void SwitchState(GameState state)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = state;
            CurrentState.Enter();
            _context.GameStateTransitionEvent.RaiseEvent(_context, PreviousState.GetEnum(), state.GetEnum());
        }

        public void SwitchStateUpdateOnly(GameState state)
        {
            PreviousState = CurrentState;
            CurrentState = state;
            _context.GameStateTransitionEvent.RaiseEvent(_context, PreviousState.GetEnum(), state.GetEnum());
        }

        public void EnterLoading()
        {
            if (CurrentState == LoadingState) return;
            CurrentState.EnterLoading();
            _inLoadingState = CurrentState;
            CurrentState = LoadingState;
            LoadingState.Enter();
            _context.GameStateTransitionEvent.RaiseEvent(_context, _inLoadingState.GetEnum(), LoadingState.GetEnum());
        }

        public void ExitLoading()
        {
            if (CurrentState != LoadingState) return;
            LoadingState.Exit();
            CurrentState = _inLoadingState;
            _inLoadingState.ExitLoading();
            _inLoadingState = null;
            _context.GameStateTransitionEvent.RaiseEvent(_context, GameStatesEnum.Loading, CurrentState.GetEnum());
        }

        public void SwitchToNarration(DialogData data)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = NarrationState;
            NarrationState.EnterNarration(data);
        }

        public void SwitchToHud(HudEnum hud)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = HudState;
            HudState.EnterHud(hud);
        }

        public void Update()
        {
            CurrentState?.Update();
        }
    }
}
