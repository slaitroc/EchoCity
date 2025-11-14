using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string firstLevelName = "Noah's Lab";
    [SerializeField] private string debugSceneName = "none";
    private string _currentLevelName;
    private void Start()
    {
        StartCoroutine(LoadLevelAdditive(firstLevelName));
        if (debugSceneName != "none") StartCoroutine(LoadSceneAdditiveNoActive(debugSceneName));
    }
    public IEnumerator LoadLevelAdditive(string sceneName)
    {
#if UNITY_EDITOR
        Scene existingScene = SceneManager.GetSceneByName(sceneName);
        if (existingScene.IsValid() && existingScene.isLoaded)
        {
            _currentLevelName = sceneName;
            SceneManager.SetActiveScene(existingScene);
            PlacePlayerOnSpawn();
            Log.D("Scene already loaded in editor, just activated: " + sceneName, "yellow", "SceneLoader");
            yield break;
        }
#endif
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = true;

        while (!op.isDone)
        {
            //NOTE Loading bar here
            yield return null;
        }

        _currentLevelName = sceneName;
        Scene levelScene = SceneManager.GetSceneByName(sceneName);
        if (levelScene.IsValid())
        {
            SceneManager.SetActiveScene(levelScene);
        }

        PlacePlayerOnSpawn();
        Log.D("Loaded active scene: " + sceneName, "red", "SceneLoader");
    }

    public void PlacePlayerOnSpawn()
    {
        GameObject spawn = GameObject.FindWithTag("Respawn");
        GameObject player = GameObject.FindWithTag("Player");

        if (spawn != null && player != null)
        {
            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;
        }

    }

    public IEnumerator LoadSceneAdditiveNoActive(string sceneName)
    {
#if UNITY_EDITOR
        Scene existingScene = SceneManager.GetSceneByName(sceneName);
        if (existingScene.IsValid() && existingScene.isLoaded)
        {
            Log.D("Scene already loaded in editor, just activated: " + sceneName, "yellow", "SceneLoader");
            yield break;
        }
#endif
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!op.isDone)
        {
            yield return null;
        }

        Log.D("Loaded non-active scene: " + sceneName, "red", "SceneLoader");
    }






}
