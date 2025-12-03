using UnityEngine;

public class GameManager : MonoBehaviour
{
#pragma warning disable CS0414
    private string _LOG_TAG = "GAME MANAGER";
    private string _LOG_COLOR = "#00ff00ff";
#pragma warning restore CS0414

    [Header("Invoking Events")]
    [SerializeField] private SOEventDoubleParam<GameStatesEnum, GameStatesEnum> switchGameStateEvent;

    private GameStatesFSM _fsm;
    public GameStatesEnum CurrentState;

    void Awake()
    {
        _fsm = new GameStatesFSM(this);
        _fsm.Initialize();
    }

    void Update() => _fsm.Update();
    public void RaiseSwitchStateEvent(GameStatesEnum from, GameStatesEnum to) => switchGameStateEvent.RaiseEvent(from, to);
    public void PauseGameHandler() => _fsm.CurrentState.PauseGameHandler();

}