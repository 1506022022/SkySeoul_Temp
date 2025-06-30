using UnityEngine.ResourceManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class Walk : IMove, IDirection, IStrength, IUpdateReceiver
    {
        float strength;
        Transform actor;
        Vector3 direction;
        NavMeshAgent agent;


        void IMove.SetActor(Transform transform)
        {
            this.actor = transform;
            agent = actor.GetComponent<NavMeshAgent>();
        }
        void IDirection.SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }
        void IStrength.SetStrength(float strength)
        {
            this.strength = strength;
        }
        float IStrength.GetStrength()
        {
            return strength;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (actor == null || agent == null || !agent.enabled) return;
            var dir = direction;
            SetDirOfForward(ref dir);
            agent.Move(dir * strength * unscaledDeltaTime);
            strength *= 1f - Mathf.Clamp01(1.5f * unscaledDeltaTime);
            strength = strength < 0.1f ? 0f : strength;
        }
        void SetDirOfForward(ref Vector3 dir)
        {
            if (actor == null) return;

            var @base = actor.transform;
            dir = @base.forward * dir.z + @base.right * dir.x;
            dir.y = 0;
            dir = dir.normalized;
        }
    }
    public class Jump : IMove, IStrength, IUpdateReceiver
    {
        float strength;
        Transform actor;
        NavMeshAgent agent;

        void IMove.SetActor(Transform transform)
        {
            this.actor = transform;
            agent = actor.GetComponent<NavMeshAgent>();
        }
        public void SetStrength(float strength)
        {
            this.strength = strength;

            if (!actor || !agent) return;
            agent.enabled = false;
            actor.position += Vector3.up * strength;
        }
        float IStrength.GetStrength()
        {
            return strength;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (actor == null || agent == null) return;

            var checker = actor.GetComponent<IGroundCheckable>();
            agent.enabled = checker?.IsGrounded ?? true;
            agent.enabled = agent.isOnNavMesh;
            strength = agent.isOnNavMesh ? 0 : strength;
        }
    }
    public class Sliding : IMove, IDirection, IStrength, IUpdateReceiver
    {
        float strength;
        float startStrength;
        float startTime;
        Vector3 direction;
        Transform actor;
        NavMeshAgent agent;

        void IMove.SetActor(Transform transform)
        {
            this.actor = transform;
            agent = actor.GetComponent<NavMeshAgent>();
        }
        void IDirection.SetDirection(Vector3 direction)
        {
            this.direction = direction;
        }
        void IStrength.SetStrength(float strength)
        {
            this.startStrength = strength;
            this.strength = strength;
            this.startTime = Time.time;
        }
        float IStrength.GetStrength()
        {
            return strength;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (!actor || !agent || strength == 0) return;
            var t = Mathf.Clamp01(Time.time - startTime);
            strength = startStrength * (1 - t);
            agent.Move(strength * unscaledDeltaTime * direction);
        }
    }
    public class ReciveGravity : IMove, IUpdateReceiver
    {
        Transform actor;
        void IMove.SetActor(Transform transform)
        {
            this.actor = transform;
        }
        void IUpdateReceiver.Update(float unscaledDeltaTime)
        {
            if (actor == null) return;
            var checker = actor.GetComponent<IGroundCheckable>();
            var grounded = checker?.IsGrounded ?? true;
            if (!grounded) actor.transform.position += Physics.gravity * unscaledDeltaTime;
        }
    }
}