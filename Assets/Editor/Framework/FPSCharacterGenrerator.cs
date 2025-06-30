using Battle;
using Character;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class FPSCharacterGenrerator : Generator<FPSCharacterGenrerator>
{
    protected override string folderPath => "Assets/Runtime/PlayableCharacter";
    protected override uint guid => 2000000000;
    protected override string basePrefabName => "FPSCharacter";
    protected override string addressableLabel => nameof(IPlayable);

    [Header("Require")]
    public GameObject Model;

    [Header("Override")]
    public RuntimeAnimatorController Animator;

    [MenuItem("Assets/Create/FPSCharacter")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<FPSCharacterGenrerator>("Create FPSCharacter", "Create");
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
        animator.applyRootMotion = false;

        var ec = go.GetComponent<CharacterBaseComponent>();
        FieldInfo fieldInfo = ec.GetType().GetField("animator", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        fieldInfo?.SetValue(ec, animator);
    }

    protected override bool IsValid()
    {
        return true;
    }
}
