using System.IO;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;
using Battle;

public abstract class Generator<T> : ScriptableWizard
{
    protected abstract string folderPath { get; }
    protected abstract uint guid { get; }
    protected abstract string basePrefabName { get; }
    protected abstract string addressableLabel { get; }
    protected abstract void InitializePrefab(GameObject go);
    protected abstract bool IsValid();
    protected void OnWizardCreate()
    {
        if (IsValid()) GeneratePrefab(this);
    }
    protected void OnWizardUpdate()
    {
        helpString = $"is Ready : {IsValid().ToString()}";
    }
    protected void OnWizardOtherButton()
    {
        
    }
    public void GeneratePrefab(Generator<T> generator)
    {
        RequireDirectory(generator.folderPath);

        string[] files = Directory.GetFiles(generator.folderPath, "*.prefab", SearchOption.TopDirectoryOnly);
        int count = files?.Count(x => x.EndsWith(".prefab")) ?? 0;
        string prefabName = $"{(generator.guid + count):D10}";
        string prefabPath = generator.folderPath + $"/{prefabName}.prefab";
        var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Editor/BasePrefab/{generator.basePrefabName}.prefab");
        GameObject go = GameObject.Instantiate(basePrefab);
        generator.InitializePrefab(go);
        var success = SaveAsPrefab(prefabPath, go);
        SetAddressableLabel(prefabPath, addressableLabel);
        GameObject.DestroyImmediate(go);

        Object prefabAsset = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
        if (success) EditorGUIUtility.PingObject(prefabAsset);
        else Debug.LogError("Failed to create");
    }

    private static void RequireDirectory(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }
    }

    static bool SaveAsPrefab(string prefabPath, GameObject go)
    {
        PrefabUtility.SaveAsPrefabAsset(go, prefabPath, out bool success);
        return success;
    }


    static void SetAddressableLabel(string prefabPath, string label)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return;
        }

        string assetGUID = AssetDatabase.AssetPathToGUID(prefabPath);

        AddressableAssetEntry entry = settings.FindAssetEntry(assetGUID);
        if (entry == null)
        {
            AddressableAssetGroup group = settings.DefaultGroup;
            entry = settings.CreateOrMoveEntry(assetGUID, group);
        }

        if (!settings.GetLabels().Contains(label))
        {
            settings.AddLabel(label);
        }
        entry.SetLabel(label, true);
    }
}