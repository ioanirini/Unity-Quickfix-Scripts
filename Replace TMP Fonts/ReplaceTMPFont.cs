using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ReplaceTMPFont : EditorWindow
{
    public TMP_FontAsset newFont;

    [MenuItem("Tools/Replace TMP Font")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceTMPFont>("Replace TMP Font");
    }

    private void OnGUI()
    {
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Font in All Scenes and Prefabs"))
        {
            if (newFont != null)
            {
                ReplaceFontInAllScenes();
                ReplaceFontInAllPrefabs();
            }
            else
            {
                Debug.LogError("Please assign a new TMP font.");
            }
        }
    }

    private void ReplaceFontInAllScenes()
    {
      
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            ReplaceFontInScene(scene);
        }

        // For any unloaded scenes that might be part of the build, load them
        string[] scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        foreach (var path in scenePaths)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(path);
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            ReplaceFontInScene(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private void ReplaceFontInScene(Scene scene)
    {
        
        if (!scene.isLoaded)
            return;
 
        TMP_Text[] tmpTexts = FindObjectsOfType<TMP_Text>(true); // true to include inactive objects
       
        foreach (var tmp in tmpTexts)
        {
            tmp.font = newFont;
        }
        // Mark the scene as dirty and save it
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private void ReplaceFontInAllPrefabs()
    {   
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });

        foreach (var path in prefabPaths)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(path);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {            
                TMP_Text[] tmpTexts = prefab.GetComponentsInChildren<TMP_Text>(true); // true to include inactive children
                
                foreach (var tmp in tmpTexts)
                {
                    tmp.font = newFont;
                }                
                PrefabUtility.SavePrefabAsset(prefab);
            }
        }      
        AssetDatabase.Refresh();
    }
}