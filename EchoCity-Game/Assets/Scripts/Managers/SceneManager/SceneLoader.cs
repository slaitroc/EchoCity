using System.Collections;
using EchoCity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoCity
{
    public class SceneLoader : MonoBehaviour, IEventSender
    {
        [Header("Invoking Events")]
        [SerializeField] private SOSwitchToGameStateEvent switchToGameStateEvent;
        [SerializeField] private SOSetMaterialEvent setMaterialEvent;

        string IEventSender.SenderName => gameObject.name;
        int IEventSender.SenderID => GetInstanceID();
        bool IEventSender.IsManager => false;
        EventSenderCategoriesEnum[] IEventSender.SenderCategory => new EventSenderCategoriesEnum[] { EventSenderCategoriesEnum.SceneLoader };

        [Header("Observed Events")]
        [SerializeField] private SOLevelActionEvent loadLevelEvent;
        [SerializeField] private SOSetPlayerOnSpawnEvent setPlayerOnSpawnEvent;

        [Header("Settings")]
        [SerializeField]
        private string[] scenesNames ={
        "None",
        "Persistent",
        "Playground",
        "First-Level",
        "Second-Level"
    };
        private SceneEnum _currentLevelEnum = SceneEnum.None;

        private void OnEnable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised += LevelActionHandler;
            if (setPlayerOnSpawnEvent) setPlayerOnSpawnEvent.OnEventRaised += SetPlayerOnSpawnHandler;
        }

        private void OnDisable()
        {
            if (loadLevelEvent) loadLevelEvent.OnEventRaised -= LevelActionHandler;
            if (setPlayerOnSpawnEvent) setPlayerOnSpawnEvent.OnEventRaised -= SetPlayerOnSpawnHandler;
        }

        public void SetPlayerOnSpawnHandler(IEventSender sender)
        {
            StartCoroutine(SetPlayerOnSpawnCoroutine());
        }

        private IEnumerator SetPlayerOnSpawnCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            var _player = GameObject.FindWithTag("Player");
            var _spawnPoint = GameObject.FindWithTag("Respawn")?.transform;
            var pc = _player?.GetComponent<PlayerController>();
            var cc = _player?.GetComponent<CharacterController>();


            if (_spawnPoint != null && _player != null && pc != null)
            {
                if (cc != null) cc.enabled = false;

                _player.transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);
                pc.currentHealth = pc.maxHealth;

                yield return null;

                if (cc != null) cc.enabled = true;

                Log.DLazy(() => $"Player respawned at {_spawnPoint.position}", this);
            }
            else
            {
                Log.WLazy(() => "Failed to find Player or Respawn point", this);
            }
        }

        private void LevelActionHandler(IEventSender sender, LevelActionCodeEnum code, SceneEnum scene)
        {
            switch (code)
            {
                case LevelActionCodeEnum.LoadActiveLevel:
                    StartCoroutine(LoadLevelAdditiveWithLoading(scene));
                    break;
                case LevelActionCodeEnum.LoadLevel:
                    StartCoroutine(LoadSceneAdditiveNoActiveWithLoading(scene));
                    break;
                case LevelActionCodeEnum.UnloadLevel:
                    StartCoroutine(UnloadCurrentLevelWithLoading());
                    break;
                case LevelActionCodeEnum.ReloadLevel:
                    StartCoroutine(ReloadCurrentLevelWithLoading());
                    break;
                default:
                    break;
            }
        }

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
                Log.DLazy(() => "Scene already loaded in editor, just activated: " + sceneName, this);
                if (_currentLevelEnum == SceneEnum.Level1)
                {
                    setMaterialEvent?.RaiseEvent(this, EchoMaterialCodeEnum.Active);
                }
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
            Log.DLazy(() => "Loaded active scene: " + sceneName, this);
            if (_currentLevelEnum == SceneEnum.Level1)
            {
                setMaterialEvent?.RaiseEvent(this, EchoMaterialCodeEnum.Active);
            }
        }

        public IEnumerator LoadSceneAdditiveNoActive(SceneEnum scene)
        {
            string sceneName = scenesNames[(int)scene];
#if UNITY_EDITOR
            Scene existingScene = SceneManager.GetSceneByName(sceneName);
            if (existingScene.IsValid() && existingScene.isLoaded)
            {
                Log.DLazy(() => "Scene already loaded in editor, just activated: " + sceneName, this);
                yield break;
            }
#endif
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!op.isDone)
            {
                yield return null;
            }

            Log.DLazy(() => "Loaded non-active scene: " + sceneName, this);
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
                        Log.DLazy(() => "Unloaded scene: " + sceneName, this);
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
            SetPlayerOnSpawnHandler(this);
        }

        private IEnumerator UnloadCurrentLevelWithLoading()
        {
            yield return StartCoroutine(StartLoading());
            yield return StartCoroutine(UnloadOtherLevels(SceneEnum.None));
            yield return StartCoroutine(StopLoading());
        }

        private IEnumerator StartLoading()
        {
            switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Loading, new ToLoadingStateParams(true));
            yield return new WaitForSecondsRealtime(0.5f);
        }

        private IEnumerator StopLoading()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            switchToGameStateEvent?.RaiseEvent(this, GameStatesEnum.Loading, new ToLoadingStateParams(false));
        }
    }
}
