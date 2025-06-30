using BehaviorDesigner.Runtime.Tactical;
using Character;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using IDamageable = Character.IDamageable;

namespace Battle
{
    public class EmptyJoycon : IController
    {
        public EmptyJoycon(IActor actor)
        {
            JoinCharacter(actor);
        }

        public void Update()
        {
        }

        public void JoinCharacter(IActor actor)
        {
            if (actor is IHitBox body) body.HitBox.OnCollision += OnHitCharacter;
        }
        void OnHitCharacter(HitBoxCollision collision)
        {
            collision.Victim.Actor.TryGetComponent<IHP>(out var health);
            collision.Victim.Actor.TryGetComponent<IDamageable>(out var hit);
            collision.Victim.Actor.TryGetComponent<IDeathable>(out var death);

            if (hit == null && death == null) return;

            if (health == null) hit?.TakeDamage();
            else if (health.HP.Value > 0) hit?.TakeDamage(); else death?.Die();
        }
    }

    public class MonsterMovement : IMovement
    {
        readonly NavMeshAgent agent;
        readonly CharacterState character;

        public float speed = 3;
        public bool IsGrounded => agent?.isOnNavMesh ?? true;

        public MonsterMovement(CharacterState character, Transform transform)
        {
            this.character = character;
            character.OnMove += Move;
            character.OnRun += Run;
            if (!transform.TryGetComponent(out agent)) agent = transform.AddComponent<NavMeshAgent>();
        }

        void Move(Vector3 position)
        {
            agent.SetDestination(position);
            agent.speed = speed;
        }
        void Run(Vector3 position)
        {
            agent.SetDestination(position);
            agent.speed = speed * 1.5f;
        }

        public void UpdateGravity()
        {
        }
    }

    public class MonsterComponent : CharacterComponent, IEnemy
    {
        public Henchmen henchmen { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            henchmen ??= new(this);
            SetAnimator(new ZombieAnimator());
            SetMovement(new MonsterMovement(character, transform));
            SetController(henchmen);
            henchmen.Team = Team.Monster;

            StartCoroutine(WaitAndActiveAgent());
        }
        IEnumerator WaitAndActiveAgent()
        {
            yield return new WaitForSeconds(1f);
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
