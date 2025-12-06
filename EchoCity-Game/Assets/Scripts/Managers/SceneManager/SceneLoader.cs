using System.Collections;
using EchoCity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoCity
{
    public class SceneLoader : MonoBehaviour
    {
        private string _LOG_TAG = "SCENE LOADER";
        private string _LOG_COLOR = "#ed600eff";

        [Header("Invoking Events")]
        [SerializeField] private SOEventVoid loading;
        [SerializeField] private SOEventVoid loadDoneEvent;
        // [SerializeField] private SOEventVoid unloadDoneEvent;

        [Header("Observed Events")]
        [SerializeField] private SOSceneEnumEvent loadLevelEvent;
        [SerializeField] private SOEventVoid unloadCurrentLevelEvent;
        [SerializeField] private SOEventVoid reloadLevelEvent;

        [Header("Settings")]
        [SerializeField]
        private string[] scenesNames ={
        "None",
        "Persistent",
        "First-Level",
        "Second-Level",
        "Third-Level"
    };
        private SceneEnum _currentLevelEnum = SceneEnum.None;


        private void OnEnable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised += LoadLevelAdditiveHandler;
            if (unloadCurrentLevelEvent) unloadCurrentLevelEvent.OnEventRaised += UnloadCurrentLevelHandler;
            if (reloadLevelEvent) reloadLevelEvent.OnEventRaised += ReloadCurrentLevelHandler;

        }

        private void OnDisable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised -= LoadLevelAdditiveHandler;
            if (unloadCurrentLevelEvent) unloadCurrentLevelEvent.OnEventRaised -= UnloadCurrentLevelHandler;
            if (reloadLevelEvent) reloadLevelEvent.OnEventRaised -= ReloadCurrentLevelHandler;
        }


        public void PlacePlayerOnSpawn()
        {
            GameObject spawn = GameObject.FindWithTag("Respawn");
            GameObject player = GameObject.FindWithTag("Player");
            PlayerController pc = player?.GetComponent<PlayerController>();

            if (spawn != null && player != null && pc != null)
            {
                player.transform.position = spawn.transform.position;
                player.transform.rotation = spawn.transform.rotation;
                pc.currentHealth = pc.maxHealth;
            }

        }
        public void LoadLevelAdditiveHandler(SceneEnum scene) => StartCoroutine(LoadLevelAdditive(scene));
        public void LoadSceneAdditiveNoActiveHandler(SceneEnum scene) => StartCoroutine(LoadSceneAdditiveNoActive(scene));
        public void ReloadCurrentLevelHandler() => StartCoroutine(ReloadCurrentLevel());
        public void UnloadCurrentLevelHandler() => StartCoroutine(UnloadOtherLevelsWithLoading(SceneEnum.None));
        public IEnumerator LoadLevelAdditive(SceneEnum scene)
        {
            loading?.RaiseEvent();
            string sceneName = scenesNames[(int)scene];

#if UNITY_EDITOR
            Scene existingScene = SceneManager.GetSceneByName(sceneName);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                _currentLevelEnum = scene;
                SceneManager.SetActiveScene(existingScene);
                yield return StartCoroutine(UnloadOtherLevels(scene));
                PlacePlayerOnSpawn();
                Log.D("Scene already loaded in editor, just activated: " + sceneName, $"{_LOG_COLOR}", $"{_LOG_TAG}");
                yield break;
            }
#endif

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.allowSceneActivation = true;

            while (!op.isDone)
            {
                yield return null;
            }
            yield return StartCoroutine(UnloadOtherLevels(scene));

            _currentLevelEnum = scene;
            Scene levelScene = SceneManager.GetSceneByName(sceneName);
            if (levelScene.IsValid())
            {
                SceneManager.SetActiveScene(levelScene);
                _currentLevelEnum = scene;
            }

            PlacePlayerOnSpawn();
            Log.D("Loaded active scene: " + sceneName, $"{_LOG_COLOR}", $"{_LOG_TAG}");
            loadDoneEvent?.RaiseEvent();
        }

        public IEnumerator LoadSceneAdditiveNoActive(SceneEnum scene)
        {
            loading?.RaiseEvent();
            string sceneName = scenesNames[(int)scene];
#if UNITY_EDITOR
            Scene existingScene = SceneManager.GetSceneByName(sceneName);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                Log.D("Scene already loaded in editor, just activated: " + sceneName, $"{_LOG_COLOR}", $"{_LOG_TAG}");
                yield break;
            }
#endif
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!op.isDone)
            {
                yield return null;
            }

            Log.D("Loaded non-active scene: " + sceneName, $"{_LOG_COLOR}", $"{_LOG_TAG}");
            loadDoneEvent?.RaiseEvent();
        }

        public IEnumerator ReloadCurrentLevel()
        {
            loading?.RaiseEvent();
            var existingScene = SceneManager.GetSceneByName(scenesNames[(int)_currentLevelEnum]);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(existingScene);
                while (!op.isDone)
                {
                    yield return null;
                }
            }
            StartCoroutine(LoadLevelAdditive(_currentLevelEnum));
        }

        private IEnumerator UnloadOtherLevels(SceneEnum levelToKeep)
        {

            for (int i = 0; i < scenesNames.Length; i++)
            {
                SceneEnum sceneEnum = (SceneEnum)i;
                if (sceneEnum != SceneEnum.Persistent && sceneEnum != levelToKeep)
                {
                    string sceneName = scenesNames[i];
                    Scene existingScene = SceneManager.GetSceneByName(sceneName);
                    if (existingScene.IsValid() && existingScene.isLoaded)
                    {
                        AsyncOperation op = SceneManager.UnloadSceneAsync(existingScene);
                        while (!op.isDone)
                        {
                            yield return null;
                        }
                        Log.D("Unloaded scene: " + sceneName, $"{_LOG_COLOR}", $"{_LOG_TAG}");
                    }
                }
            }
        }

        private IEnumerator UnloadOtherLevelsWithLoading(SceneEnum levelToKeep)
        {
            loading?.RaiseEvent();
            yield return StartCoroutine(UnloadOtherLevels(levelToKeep));
            loadDoneEvent?.RaiseEvent();
        }
    }
}
