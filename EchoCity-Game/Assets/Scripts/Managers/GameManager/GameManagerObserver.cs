using UnityEngine;
namespace EchoCity
{
    [RequireComponent(typeof(GameManager))]
    public class GameManagerObserver : MonoBehaviour
    {

        [Header("Observed Events")]
        [SerializeField] private SOEventVoid pauseGameEvent;
        [SerializeField] private SOEventVoid openRadialMenuEvent;
        [SerializeField] private SOEventVoid closeRadialMenuEvent;
        [SerializeField] private SOEventVoid deathEvent;

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

            if (openRadialMenuEvent)
            {
                openRadialMenuEvent.OnEventRaised -= gameManager.PauseGameHandler;
                openRadialMenuEvent.OnEventRaised += gameManager.PauseGameHandler;
            }

            if (closeRadialMenuEvent)
            {
                closeRadialMenuEvent.OnEventRaised -= gameManager.PauseGameHandler;
                closeRadialMenuEvent.OnEventRaised += gameManager.PauseGameHandler;
            }

            if (deathEvent)
            {
                deathEvent.OnEventRaised -= gameManager.DeathHandler;
                deathEvent.OnEventRaised += gameManager.DeathHandler;
            }
        }

        void OnDisable()
        {
            if (pauseGameEvent) pauseGameEvent.OnEventRaised -= gameManager.PauseGameHandler;
            if (openRadialMenuEvent) openRadialMenuEvent.OnEventRaised -= gameManager.PauseGameHandler;
            if (closeRadialMenuEvent) closeRadialMenuEvent.OnEventRaised -= gameManager.PauseGameHandler;
            if (deathEvent) deathEvent.OnEventRaised -= gameManager.DeathHandler;
        }
    }
}