using UnityEditor;
using UnityEngine;

public class SavePlayModeObjects : MonoBehaviour
{
    [MenuItem("Tools/Save Play Mode Objects")]
    public static void SaveObjects()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Disconnected)
            {
                // Save changes to the prefab
                PrefabUtility.SaveAsPrefabAsset(obj, "Assets/SavedObjects/" + obj.name + ".prefab");
                Debug.Log("Saved: " + obj.name);
            }
        }
    }
}
