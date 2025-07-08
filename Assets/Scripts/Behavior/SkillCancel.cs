using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SkillCancel", story: "[actor] cancel the skill", category: "Action/Skill", id: "ecf7eb4cf55473b5964ab7822ad38ecc")]
public partial class SkillCancel : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Actor;

    protected override Status OnStart()
    {
        if (!Actor.Value.TryGetComponent<ICaster>(out var caster)) return Status.Failure;

        caster.Cancel();
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

