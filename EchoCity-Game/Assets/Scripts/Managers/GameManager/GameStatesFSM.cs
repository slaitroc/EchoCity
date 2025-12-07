using System;
using System.Collections.Generic;

namespace EchoCity
{
    [Serializable]
    public class GameStatesFSM
    {
#pragma warning disable CS0414
        private readonly string _LOG_TAG = "GM-FSM";
        private readonly string _LOG_COLOR = "green";
#pragma warning restore CS0414

        private readonly GameManager _gameManager;
        public GameState CurrentState { get; private set; }
        public GameState PreviousState { get; private set; }
        private GameState _inLoadingState;
        public readonly GameState LoadingState;
        public readonly GameState TitleState;
        public readonly GameState InitLevelState;
        public readonly GameState PlayingState;
        public readonly GameState PauseState;
        public readonly HudGameState HudState;
        public readonly NarrationGameState NarrationState;
        public readonly GameState DeathState;

        public GameStatesFSM(GameManager gameManager)
        {
            LoadingState = new LoadingGameState(gameManager, this);
            TitleState = new TitleGameState(gameManager, this);
            InitLevelState = new InitLevelGameState(gameManager, this);
            PlayingState = new PlayingGameState(gameManager, this);
            PauseState = new PauseGameState(gameManager, this);
            NarrationState = new NarrationGameState(gameManager, this);
            DeathState = new DeathGameState(gameManager, this);
            HudState = new HudGameState(gameManager, this);
            _gameManager = gameManager;
        }

        public void Initialize()
        {
            CurrentState = TitleState;
            CurrentState.Enter();
            _gameManager.RaiseSwitchStateEvent(GameStatesEnum.None, GameStatesEnum.Title);
        }
        public void Initialize(GameState state)
        {
            CurrentState = state;
            CurrentState.Enter();
            _gameManager.RaiseSwitchStateEvent(GameStatesEnum.None, state.GetEnum());
        }


        public void SwitchState(GameState state)
        {
            CurrentState.Exit();
            PreviousState = CurrentState;
            CurrentState = state;
            CurrentState.Enter();
            _gameManager.RaiseSwitchStateEvent(PreviousState.GetEnum(), state.GetEnum());
        }

        public void SwitchStateUpdateOnly(GameState state)
        {
            PreviousState = CurrentState;
            CurrentState = state;
            _gameManager.RaiseSwitchStateEvent(PreviousState.GetEnum(), state.GetEnum());
        }

        public void EnterLoading()
        {
            if (CurrentState == LoadingState) return;
            CurrentState.EnterLoading();
            _inLoadingState = CurrentState;
            CurrentState = LoadingState;
            LoadingState.Enter();
            _gameManager.RaiseSwitchStateEvent(_inLoadingState.GetEnum(), LoadingState.GetEnum());
        }

        public void ExitLoading()
        {
            if (CurrentState != LoadingState) return;
            LoadingState.Exit();
            CurrentState = _inLoadingState;
            _inLoadingState.ExitLoading();
            _inLoadingState = null;
            _gameManager.RaiseSwitchStateEvent(GameStatesEnum.Loading, CurrentState.GetEnum());
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
