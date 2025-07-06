using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace Framework
{
    public class SkillGenerator
    {
        static readonly string folderPath = "Assets/Runtime/Skill";
        const uint skillGUID = 2300000000;

        [MenuItem("Assets/Create/Skill")]
        public static void GenerateSkill()
        {
            RequireSkillDirectory();

            string[] files = Directory.GetFiles(folderPath, "*.prefab", SearchOption.TopDirectoryOnly);
            int count = files?.Count(x => x.EndsWith(".prefab")) ?? 0;
            string prefabName = $"{(skillGUID + count):D10}";
            string prefabPath = folderPath + $"/{prefabName}.prefab";

            GameObject skillGO = new GameObject(prefabName);
            InitializeSkillObject(skillGO);
            var success = SaveSkillObjectAsPrefab(prefabPath, skillGO);
            SetAddressableLabelOfSkillPrefab(prefabPath);
            GameObject.DestroyImmediate(skillGO);

            Object prefabAsset = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
            if (success) EditorGUIUtility.PingObject(prefabAsset);
            else Debug.LogError("Failed to create skill prefab.");
        }

        private static void RequireSkillDirectory()
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
        }

        static bool SaveSkillObjectAsPrefab(string prefabPath, GameObject skillGO)
        {
            PrefabUtility.SaveAsPrefabAsset(skillGO, prefabPath, out bool success);
            return success;
        }

        static void InitializeSkillObject(GameObject skillGO)
        {
            skillGO.AddComponent<SkillComponent>();
        }

        static void SetAddressableLabelOfSkillPrefab(string prefabPath)
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

            if (!settings.GetLabels().Contains(nameof(Skill)))
            {
                settings.AddLabel(nameof(Skill));
            }
            entry.SetLabel(nameof(Skill), true);
        }
    }
}