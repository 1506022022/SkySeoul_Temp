using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Character;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Travel", story: "Where [travelers] go [start]", category: "Action", id: "65bea4532a08f76b5494a4abd82e640d")]
public partial class Travel : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Travelers;
    [SerializeReference] public BlackboardVariable<bool> Start;

    protected override Status OnStart()
    {
        if (!Travelers.Value.TryGetComponent<ITraveler>(out var traveler)) return Status.Failure;
        if (Start) traveler.StartTravel(); else traveler.EndTravel();

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