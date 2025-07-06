using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AutoWaypoints", story: "get [waypoints] by [actor]", category: "Action", id: "5f0681f34dbb5efc5b06f7b6213360da")]
public partial class AutoWaypoints : Action
{
    [SerializeReference] public BlackboardVariable<Transform> actor;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;
    [SerializeReference] public BlackboardVariable<float> radius;
    [SerializeReference] public BlackboardVariable<int> count;
    Vector3 originPos;

    void GetRandomNavMeshPositions()
    {
        for (int i = 0; i < Waypoints.Value.Count; i++)
        {
            GameObject.Destroy(Waypoints.Value[i]);
        }
        Waypoints.Value.Clear();
        uint exitCount = 0;

        for (int i = 0; i < count.Value; i++)
        {
            if (300 < exitCount) break;

            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * radius.Value;
            randomDirection += originPos;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius.Value, NavMesh.AllAreas))
            {
                var go = new GameObject("waypoint");
                go.transform.position = hit.position;
                Waypoints.Value.Add(go);
            }
            else
            {
                i--;
                exitCount++;
            }
        }
    }

    protected override Status OnStart()
    {
        if (actor == null || Waypoints == null) return Status.Failure;
        if (originPos == Vector3.zero) originPos = actor.Value.position;

        GetRandomNavMeshPositions();

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

