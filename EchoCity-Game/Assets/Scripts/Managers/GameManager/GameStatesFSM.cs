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
        public readonly GameState PlayingState;
        public readonly GameState PauseState;
        public readonly GameState NarrationState;
        public readonly GameState DeathState;

        public GameStatesFSM(GameManager gameManager)
        {
            PlayingState = new PlayingGameState(gameManager, this);
            PauseState = new PauseGameState(gameManager, this);
            NarrationState = new NarrationGameState(gameManager, this);
            DeathState = new DeathState(gameManager, this);
            _gameManager = gameManager;
        }

        public void Initialize()
        {
            CurrentState = PlayingState;
            CurrentState.Enter();
            _gameManager.RaiseSwitchStateEvent(GameStatesEnum.None, GameStatesEnum.Playing);
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

        public void Update()
        {
            CurrentState?.Update();
        }
    }
}
