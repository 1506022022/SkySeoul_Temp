using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HasAbnormalStatus", story: "set [has] [actor] [abnormalStatus]", category: "Action/Get", id: "bc21213f4a9cbfb7e54530145c57e088")]
public partial class HasAbnormalStatus : Action
{
    [SerializeReference] public BlackboardVariable<bool> Has;
    [SerializeReference] public BlackboardVariable<Transform> Actor;
    [SerializeReference] public BlackboardVariable<AbnormalStatus> AbnormalStatus;

    protected override Status OnStart()
    {
        switch (AbnormalStatus.Value)
        {
            case global::AbnormalStatus.HitStun:
                if (!Actor.Value.TryGetComponent<IHitStun>(out var hitStun)) break;
                Has.Value = hitStun.IsHit;
                break;
            case global::AbnormalStatus.Stun:
                if (!Actor.Value.TryGetComponent<IStun>(out var stun)) break;
                Has.Value = stun.IsStun;
                break;
            case global::AbnormalStatus.Die:
                if (!Actor.Value.TryGetComponent<IDeathable>(out var deathable)) break;
                Has.Value = deathable.IsDead;
                break;
            default:
                Has.Value = false;
                break;
        }
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

