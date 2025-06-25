using UnityEditor;
using UnityEngine;

public class MissingScriptRemover
{
    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    static void RemoveMissingScripts()
    {
        int count = 0;

        GameObject[] allGameObjects = Object.FindObjectsByType<GameObject>(  FindObjectsSortMode.InstanceID);

        foreach (GameObject go in allGameObjects)
        {

            int removedCount = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removedCount > 0)
            {
                Debug.Log($"Removed {removedCount} missing script(s) from GameObject: {go.name}", go);
                count += removedCount;
            }
        }

        Debug.Log($"Done. Total missing scripts removed: {count}");
    }
}
