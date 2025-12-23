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
        [SerializeField] private SOEventVoid enterLoadingEvent;
        [SerializeField] private SOEventVoid exitLoadingEvent;
        // [SerializeField] private SOEventVoid unloadDoneEvent;

        [Header("Observed Events")]
        [SerializeField] private SOSceneEnumEvent loadLevelEvent;
        [SerializeField] private SOEventVoid unloadCurrentLevelEvent;
        [SerializeField] private SOEventVoid reloadLevelEvent;
        [SerializeField] private SOEventVoid setPlayerOnSpawnEvent;

        [Header("Settings")]
        [SerializeField]
        private string[] scenesNames ={
        "None",
        "Persistent",
        "First-Level",
        "Second-Level",
        "Third-Level"
    };
        private GameObject _player;
        private Transform _spawnPoint;

        private SceneEnum _currentLevelEnum = SceneEnum.None;


        private void OnEnable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised += LoadLevelAdditiveHandler;
            if (unloadCurrentLevelEvent) unloadCurrentLevelEvent.OnEventRaised += UnloadCurrentLevelHandler;
            if (reloadLevelEvent) reloadLevelEvent.OnEventRaised += ReloadCurrentLevelHandler;
            if (setPlayerOnSpawnEvent) setPlayerOnSpawnEvent.OnEventRaised += PlacePlayerOnSpawn;
        }

        private void OnDisable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised -= LoadLevelAdditiveHandler;
            if (unloadCurrentLevelEvent) unloadCurrentLevelEvent.OnEventRaised -= UnloadCurrentLevelHandler;
            if (reloadLevelEvent) reloadLevelEvent.OnEventRaised -= ReloadCurrentLevelHandler;
            if (setPlayerOnSpawnEvent) setPlayerOnSpawnEvent.OnEventRaised -= PlacePlayerOnSpawn;
        }

        public void PlacePlayerOnSpawn() //BUG
        {
            if (_player == null)
                _player = GameObject.FindWithTag("Player");
            if (_spawnPoint == null)
                _spawnPoint = GameObject.FindWithTag("Respawn")?.transform;
            PlayerController pc = _player?.GetComponent<PlayerController>();

            if (_spawnPoint != null && _player != null && pc != null)
            {
                _player.transform.position = _spawnPoint.position;
                _player.transform.rotation = _spawnPoint.rotation;
                pc.currentHealth = pc.maxHealth;
            }
        }
        public void LoadLevelAdditiveHandler(SceneEnum scene) => StartCoroutine(LoadLevelAdditiveWithLoading(scene));
        public void LoadSceneAdditiveNoActiveHandler(SceneEnum scene) => StartCoroutine(LoadSceneAdditiveNoActiveWithLoading(scene));
        public void ReloadCurrentLevelHandler() => StartCoroutine(ReloadCurrentLevelWithLoading());
        public void UnloadCurrentLevelHandler() => StartCoroutine(UnloadCurrentLevelWithLoading());

        public IEnumerator LoadLevelAdditive(SceneEnum scene)
        {
            string sceneName = scenesNames[(int)scene];
#if UNITY_EDITOR
            //if the scene is already loaded, just set it active and unload others
            Scene existingScene = SceneManager.GetSceneByName(sceneName);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                _currentLevelEnum = scene;
                SceneManager.SetActiveScene(existingScene);
                yield return StartCoroutine(UnloadOtherLevels(scene));
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
            Log.D("Loaded active scene: " + sceneName, $"{_LOG_COLOR}", $"{_LOG_TAG}");
        }

        public IEnumerator LoadSceneAdditiveNoActive(SceneEnum scene)
        {
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
        }

        public IEnumerator ReloadCurrentLevel()
        {
            var existingScene = SceneManager.GetSceneByName(scenesNames[(int)_currentLevelEnum]);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                AsyncOperation op = SceneManager.UnloadSceneAsync(existingScene);
                while (!op.isDone)
                {
                    yield return null;
                }
            }
            yield return StartCoroutine(LoadLevelAdditive(_currentLevelEnum));
        }

        // Unloads all levels except the specified one
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
            yield return StartCoroutine(StartLoading());
            yield return StartCoroutine(UnloadOtherLevels(levelToKeep));
            yield return StartCoroutine(StopLoading());
        }

        private IEnumerator LoadSceneAdditiveNoActiveWithLoading(SceneEnum scene)
        {
            yield return StartCoroutine(StartLoading());
            yield return StartCoroutine(LoadSceneAdditiveNoActive(scene));
            yield return StartCoroutine(StopLoading());
        }

        private IEnumerator LoadLevelAdditiveWithLoading(SceneEnum scene)
        {
            yield return StartCoroutine(StartLoading());
            yield return StartCoroutine(LoadLevelAdditive(scene));
            yield return StartCoroutine(StopLoading());
        }

        private IEnumerator ReloadCurrentLevelWithLoading()
        {
            yield return StartCoroutine(StartLoading());
            yield return StartCoroutine(ReloadCurrentLevel());
            yield return StartCoroutine(StopLoading());
        }

        private IEnumerator UnloadCurrentLevelWithLoading()
        {
            yield return StartCoroutine(StartLoading());
            yield return StartCoroutine(UnloadOtherLevels(SceneEnum.None));
            yield return StartCoroutine(StopLoading());
        }

        private IEnumerator StartLoading()
        {
            enterLoadingEvent?.RaiseEvent();
            yield return new WaitForSecondsRealtime(0.5f);
        }

        private IEnumerator StopLoading()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            exitLoadingEvent?.RaiseEvent();
        }
    }
}
