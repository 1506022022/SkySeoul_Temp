using System;
using UnityEngine;
using UnityEngine.AI;

namespace Battle
{
    public enum Team : byte { None, Monster, NPC, Player }
    public class Henchmen
    {
        public static event Action<Henchmen> OnSpawnEvent;
        public static event Action<Henchmen> OnDestroyEvent;
        public Team Team;
        public bool HasDependency { get; set; }
        public Vector3 Position => _transform.position;
        private readonly NavMeshAgent Agent;
        private readonly Transform _transform;

        public Henchmen(Transform transform, NavMeshAgent agent)
        {
            Agent = agent;
            _transform = transform;
        }
        public void LookForJob()
        {
            OnSpawnEvent?.Invoke(this);
        }
        public void LeaveJob()
        {
            OnDestroyEvent?.Invoke(this);
        }
        public void MoveTo(Vector3 position)
        {
            Agent.SetDestination(position);
        }
    }
}
