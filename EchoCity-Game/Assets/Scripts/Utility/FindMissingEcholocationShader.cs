#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace EchoCity.EditorTools
{
    public class FindMissingEcholocationShader : EditorWindow
    {
        private class RendererInfo
        {
            public GameObject GameObject;
            public string GameObjectName;
            public Material Material;
            public string MaterialName;
            public string CurrentShader;
            public Renderer Renderer;
        }

        private Vector2 scrollPosition;
        private List<RendererInfo> cache = new List<RendererInfo>();
        private string targetShaderName = "Custom/EcholocationLit";
        private string sourceShaderName = "Universal Render Pipeline/Lit";

        [MenuItem("Tools/EchoCity/Find Objects Without Echolocation Shader")]
        public static void ShowWindow()
        {
            GetWindow<FindMissingEcholocationShader>("Replace URP Lit Shader");
        }

        private void OnGUI()
        {
            sourceShaderName = EditorGUILayout.TextField("Source Shader:", sourceShaderName);
            targetShaderName = EditorGUILayout.TextField("Target Shader:", targetShaderName);

            EditorGUILayout.Space();

            if (GUILayout.Button("Search Loaded Scenes", GUILayout.Height(30)))
            {
                cache.Clear();

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);

                    GameObject[] rootObjects = scene.GetRootGameObjects();
                    foreach (GameObject root in rootObjects)
                    {
                        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
                        foreach (Renderer renderer in renderers)
                        {
                            foreach (Material mat in renderer.sharedMaterials)
                            {
                                if (mat == null) continue;

                                if (mat.shader == Shader.Find(sourceShaderName))
                                {
                                    cache.Add(new RendererInfo
                                    {
                                        GameObject = renderer.gameObject,
                                        GameObjectName = renderer.gameObject.name,
                                        Material = mat,
                                        MaterialName = mat.name,
                                        CurrentShader = mat.shader.name,
                                        Renderer = renderer
                                    });
                                }
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space();

            if (cache.Count > 0)
            {
                GUILayout.Label($"Found {cache.Count} objects without the shader");

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                foreach (var info in cache)
                {
                    EditorGUILayout.LabelField(info.GameObjectName);
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                if (GUILayout.Button("Fix All", GUILayout.Height(25)))
                {
                    if (!EditorUtility.DisplayDialog("Fix All Shaders",
                        $"This will change {cache.Count} materials to use '{targetShaderName}'. Continue?",
                        "Yes", "Cancel"))
                    {
                        return;
                    }

                    foreach (var info in cache.ToList())
                    {
                        Undo.RecordObject(info.Material, "Change Shader");
                        info.Material.shader = Shader.Find(targetShaderName);
                        EditorUtility.SetDirty(info.Material);
                    }

                    cache.Clear();
                }
            }
            else if (cache.Count == 0 && GUILayout.Button("Clear Results"))
            {
                cache.Clear();
            }
        }

    }
}
#endif
