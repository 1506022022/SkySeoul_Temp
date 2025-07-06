using UnityEditor;
using UnityEngine;
using Character;
using System.Reflection;
using Unity.Behavior;
using Unity.VisualScripting;

public class MonsterGnerator : Generator<MonsterGnerator>
{
    [Header("Require")]
    public GameObject Model;
    public BehaviorGraph Behavior;
    [Header("Override")]
    public RuntimeAnimatorController Animator;

    protected override string folderPath => "Assets/Runtime/Monster";
    protected override uint guid => 2100000000;
    protected override string basePrefabName => "Zombie";
    protected override string addressableLabel => "IEnemy";

    [MenuItem("Assets/Create/Monster")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<MonsterGnerator>("Create Monster", "Create");
    }

    protected override void InitializePrefab(GameObject go)
    {
        if(!go.TryGetComponent<BehaviorGraphAgent>(out var bga))
        {
            bga = go.AddComponent<BehaviorGraphAgent>();
        }
        bga.Graph = Behavior;

        var model = GameObject.Instantiate(Model); model.name = nameof(model);
        model.transform.SetParent(go.transform, false);

        if (!model.TryGetComponent<Animator>(out var animator))
        {
            animator = model.AddComponent<Animator>();
        }
        if (Animator) animator.runtimeAnimatorController = Animator;

        var ec = go.GetComponent<CharacterBaseComponent>();
        FieldInfo fieldInfo = ec.GetType().GetField("animator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        fieldInfo?.SetValue(ec, animator);
    }

    protected override bool IsValid()
    {
        return Model != null && Behavior != null && (Animator != null || Model.GetComponent<Animator>()?.runtimeAnimatorController != null);
    }
}