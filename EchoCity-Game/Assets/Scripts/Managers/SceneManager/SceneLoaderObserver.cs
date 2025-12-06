using UnityEngine;

namespace EchoCity
{
    [RequireComponent(typeof(SceneLoader))]
    public class SceneLoaderObserver : MonoBehaviour
    {
        [Header("Observed Events")]
        [SerializeField] private SOStringEvent loadLevelEvent;
        [SerializeField] private SOStringEvent loadSceneNoActiveEvent;
        [SerializeField] private SOEventVoid reloadLevelEvent;

        [Header("GO with Handlers")]
        [SerializeField] private SceneLoader sceneLoader;

        private void Awake() { TryGetComponent(out sceneLoader); }

        private void OnEnable()
        {
            if (loadLevelEvent)
            {
                loadLevelEvent.OnEventRaised -= sceneLoader.LoadLevelAdditiveEvent;
                loadLevelEvent.OnEventRaised += sceneLoader.LoadLevelAdditiveEvent;
            }
            if (loadSceneNoActiveEvent)
            {
                loadSceneNoActiveEvent.OnEventRaised -= sceneLoader.LoadSceneAdditiveNoActiveEvent;
                loadSceneNoActiveEvent.OnEventRaised += sceneLoader.LoadSceneAdditiveNoActiveEvent;
            }
            if (reloadLevelEvent)
            {
                reloadLevelEvent.OnEventRaised -= sceneLoader.ReloadCurrentLevelEvent;
                reloadLevelEvent.OnEventRaised += sceneLoader.ReloadCurrentLevelEvent;
            }
        }

        private void OnDisable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised -= sceneLoader.LoadLevelAdditiveEvent;
            if (loadSceneNoActiveEvent) loadSceneNoActiveEvent.OnEventRaised -= sceneLoader.LoadSceneAdditiveNoActiveEvent;
            if (reloadLevelEvent) reloadLevelEvent.OnEventRaised -= sceneLoader.ReloadCurrentLevelEvent;
        }
    }
}