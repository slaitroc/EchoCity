#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class FindAssetByGUIDWindow : EditorWindow
{
    private string guid = "";

    [MenuItem("Tools/Find Asset by GUID %#g")] // Ctrl/Cmd + Shift + G
    public static void OpenWindow()
    {
        GetWindow<FindAssetByGUIDWindow>("Find Asset by GUID");
    }

    private void OnGUI()
    {
        GUILayout.Label("Paste an asset GUID:", EditorStyles.boldLabel);
        guid = EditorGUILayout.TextField("GUID:", guid);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Find and Select"))
        {
            FindAsset(guid);
        }

        if (GUILayout.Button("Clear"))
        {
            guid = "";
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Paste here the GUID found in a .meta file.\n"
            + "The tool will select and ping the corresponding asset in the Project view.",
            MessageType.Info
        );
    }

    private void FindAsset(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogWarning("GUID field is empty.");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"No asset found for GUID: {guid}");
            return;
        }

        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (asset == null)
        {
            Debug.LogWarning($"Failed to load asset at path: {path}");
            return;
        }

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
        Debug.Log($"<color=cyan>Found asset:</color> {path}", asset);
    }
}
#endif
