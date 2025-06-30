using Character;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LampGenerator : Generator<LampGenerator>
{
    [Header("Require")]
    public GameObject Model;
    public SkillComponent Skill;

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

    void OnWizardCreate()
    {
        if (Valid()) GeneratePrefab(this);
    }

    protected override void InitializePrefab(GameObject go)
    {
        var model = GameObject.Instantiate(Model); model.name = nameof(model);
        model.transform.SetParent(go.transform, false);

        if (!model.TryGetComponent<Animator>(out var animator))
        {
            animator = model.AddComponent<Animator>();
        }
        if (Animator) animator.runtimeAnimatorController = Animator;

        var death = go.GetComponent<IDeathSkillOwner>();
        death.DeathSkill = Skill;
        death.DeathSkillOffset = SkillOffset;
        death.DeathSkillRotation = SkillRotation;
        FieldInfo fieldInfo = death.GetType().GetField("animator", BindingFlags.Public | BindingFlags.NonPublic |BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        fieldInfo?.SetValue(death, animator);
    }
    void OnWizardUpdate()
    {
        helpString = $"is Ready : {Valid().ToString()}";
    }
    bool Valid()
    {
        return Model != null && Skill != null && (Animator != null || Model.GetComponent<Animator>()?.runtimeAnimatorController != null);
    }
}