using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class ForceValidate
{
    [MenuItem("Tools/EchoCity/Force OnValidate All")]
    public static void ForceValidateAll()
    {
        // Recupera tutti i componenti di tipo MonoBehaviour nella scena attiva
        // inclusi quelli nei GameObject disattivati
        MonoBehaviour[] allScripts = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        int count = 0;
        foreach (var script in allScripts)
        {
            if (script != null)
            {
                // Usa la reflection per chiamare OnValidate se esiste
                var method = script.GetType().GetMethod("OnValidate", 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Public);

                if (method != null)
                {
                    method.Invoke(script, null);
                    count++;
                }
            }
        }

        Debug.Log($"[EchoCity] OnValidate forzato su {count} script con successo!");
        
        // Segna la scena come "sporca" così Unity sa che deve salvare i cambiamenti
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
}