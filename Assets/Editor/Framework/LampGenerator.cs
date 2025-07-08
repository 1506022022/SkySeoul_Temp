using Character;
using System.Reflection;
using Unity.Behavior;
using UnityEditor;
using UnityEngine;

public class LampGenerator : Generator<LampGenerator>
{
    [Header("Require")]
    public GameObject Model;
    public SkillComponent Skill;
    public BehaviorGraph Behavior;

    [Header("Override")]
    public RuntimeAnimatorController Animator;
    public Vector3 SkillOffset, SkillRotation;

    [MenuItem("Assets/Create/Lamp")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<LampGenerator>("Create Lamp", "Create");
    }

    protected override string folderPath => "Assets/Runtime/Prop";
    protected override uint guid => 2200000000;
    protected override string basePrefabName => "Lamp";
    protected override string addressableLabel => "IProp";

    protected override void InitializePrefab(GameObject go)
    {
        if (!go.TryGetComponent<BehaviorGraphAgent>(out var bga))
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

        var death = go.GetComponent<ISkillOwner>();
        death.Skill = Skill;
        death.SkillOffset = SkillOffset;
        death.SkillRotation = SkillRotation;
        FieldInfo fieldInfo = death.GetType().GetField("animator", BindingFlags.Public | BindingFlags.NonPublic |BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        fieldInfo?.SetValue(death, animator);
    }

    protected override bool IsValid()
    {
        return Model != null && Skill != null && Behavior != null && (Animator != null || Model.GetComponent<Animator>()?.runtimeAnimatorController != null);
    }
}