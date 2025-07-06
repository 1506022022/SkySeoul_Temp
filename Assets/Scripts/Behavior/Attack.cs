using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "[actor] attacks by [num]", category: "Action", id: "2b893acdb1042987e1eefa135b8ea228")]
public partial class Attack : Action
{
    [SerializeReference] public BlackboardVariable<int> Num;
    [SerializeReference] public BlackboardVariable<GameObject> actor;

    protected override Status OnStart()
    {
        if (actor?.Value != null)
            if (actor.Value.TryGetComponent<IAttackable>(out var attackable))
                attackable.Attack(Num);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

