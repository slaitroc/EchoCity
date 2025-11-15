using System;
using System.Collections.Generic;

[Serializable]
public class GameStatesFSM
{
    private string _LOG_TAG = "GM-FSM";
    private string _LOG_COLOR = "green";
    public IGameState currentState { get; private set; }
    private IGameState _playingState;
    private IGameState _pauseState;
    private IGameState _narrationState;

    private Dictionary<GameStatesEnum, IGameState> _statesDict;
    private event Action<GameStatesEnum, GameStatesEnum> OnStateChange;


    public GameStatesFSM()
    {
        _playingState = new GameStatePlaying();
        _pauseState = new GameStatePause();
        _narrationState = new GameStateNarration();

        _statesDict = new Dictionary<GameStatesEnum, IGameState>   {
            { GameStatesEnum.PLAYING,   _playingState   },
            { GameStatesEnum.PAUSE,     _pauseState     },
            { GameStatesEnum.NARRATION, _narrationState },
        };
    }

    public void Initialize()
    {
        Log.D($"Game State Initialized to {GameStatesEnum.PLAYING.ToString()}", $"{_LOG_COLOR}", $"{_LOG_TAG}");
        currentState = _statesDict[GameStatesEnum.PLAYING];
        currentState.Enter();

        OnStateChange?.Invoke(GameStatesEnum.NONE, GameStatesEnum.PLAYING);
    }
    public void Initialize(GameStatesEnum state)
    {
        Log.D($"Game State Initialized to {state.ToString()}", $"{_LOG_COLOR}", $"{_LOG_TAG}");
        currentState = _statesDict[state];
        currentState.Enter();

        OnStateChange?.Invoke(GameStatesEnum.NONE, state);
    }


    public void ChangeState(GameStatesEnum state)
    {
        Log.D($"Game State changed from {currentState.Kind()} to {state.ToString()}", $"{_LOG_COLOR}", $"{_LOG_TAG}");
        IGameState oldState = currentState;
        currentState.Exit();
        currentState = _statesDict[state];
        currentState.Enter();

        OnStateChange?.Invoke(oldState.GetEnum(), currentState.GetEnum());
    }

    public void Update()
    {
        currentState?.Update();
    }
}