using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FixMissingMaterials : MonoBehaviour
{
    [MenuItem("Tools/Fix Missing Materials (Scene + Prefabs)")]
    public static void ReplaceMissingMaterials()
    {
        // Specify the replacement material
        //Be sure to change it to your path if other than mine!
        Material replacementMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/ReplacementMaterial.mat");

        if (replacementMaterial == null)
        {
            Debug.LogError("Replacement material not found. Make sure the path is correct.");
            return;
        }        
        FixSceneMaterials(replacementMaterial);
        FixPrefabMaterials(replacementMaterial);

        Debug.Log("Finished replacing missing materials.");
    }

    private static void FixSceneMaterials(Material replacementMaterial)
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            bool hasMissingMaterial = false;

            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                if (renderer.sharedMaterials[i] == null)
                {
                    hasMissingMaterial = true;
                    break;
                }
            }

            if (hasMissingMaterial)
            {
                Debug.Log($"Fixing missing materials on {renderer.gameObject.name} in scene.");
                Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = replacementMaterial;
                }
                renderer.sharedMaterials = newMaterials;
            }
        }
    }

    private static void FixPrefabMaterials(Material replacementMaterial)
    {      
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        List<string> prefabPaths = new List<string>();

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            prefabPaths.Add(path);
        }

        foreach (string path in prefabPaths)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            bool prefabUpdated = false;

            Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                bool hasMissingMaterial = false;

                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == null)
                    {
                        hasMissingMaterial = true;
                        break;
                    }
                }

                if (hasMissingMaterial)
                {
                    Debug.Log($"Fixing missing materials on {prefab.name} (Prefab at {path}).");
                    Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
                    for (int i = 0; i < newMaterials.Length; i++)
                    {
                        newMaterials[i] = replacementMaterial;
                    }
                    renderer.sharedMaterials = newMaterials;
                    prefabUpdated = true;
                }
            }

            if (prefabUpdated)
            {              
                EditorUtility.SetDirty(prefab);
                PrefabUtility.SavePrefabAsset(prefab);
            }
        }
    }
}