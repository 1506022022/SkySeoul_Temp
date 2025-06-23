using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Battle
{
    public class EmptyJoycon : IController
    {
        public void Update()
        {
        }
    }

    public class MonsterMovement : IMovement
    {
        readonly NavMeshAgent agent;
        readonly Character character;

        public float speed = 3;
        public bool IsGrounded => agent?.isOnNavMesh ?? true;

        public MonsterMovement(Character character, Transform transform)
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
            henchmen = new(this);
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
