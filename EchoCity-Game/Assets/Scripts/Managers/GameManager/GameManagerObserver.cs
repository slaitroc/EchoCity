using UnityEngine;
[RequireComponent(typeof(GameManager))]
public class GameManagerObserver : MonoBehaviour
{

    [Header("Observed Events")]
    [SerializeField] private SOEventVoid pauseGameEvent;

    [Header("GO with Handlers")]
    [SerializeField] private GameManager gameManager;

    void Awake()
    {
        TryGetComponent(out gameManager);
    }

    void OnEnable()
    {
        if (pauseGameEvent)
        {
            pauseGameEvent.OnEventRaised -= gameManager.PauseGameHandler;
            pauseGameEvent.OnEventRaised += gameManager.PauseGameHandler;
        }
    }

    void OnDisable()
    {
        if (pauseGameEvent) pauseGameEvent.OnEventRaised -= gameManager.PauseGameHandler;
    }
}