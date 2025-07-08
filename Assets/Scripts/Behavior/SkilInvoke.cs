using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SkillInvoke", story: "[actor] uses the skill", category: "Action/Skill", id: "85b22c02fe6d7ee81f262ce284d0d642")]
public partial class SkilInvoke : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<ICaster>(out var caster)) return Status.Failure;

        caster.Invoke();
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

