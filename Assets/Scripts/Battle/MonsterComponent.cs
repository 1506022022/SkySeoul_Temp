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

    public class MonsterComponent : CharacterComponent
    {
        private Henchmen _henchmen;

        public override void Initialize()
        {
            base.Initialize();
            _henchmen = new(this);
            SetController(_henchmen);
            _henchmen.Team = Team.Monster;
        }
        protected override void Update()
        {
            base.Update();
            _henchmen.LookForJob();
        }
        void OnDisable()
        {
            _henchmen?.LeaveJob();
        }
        //protected override void OnHit(HitBoxCollision collision)
        //{
        //    base.OnHit(collision);
        //    this.StopAllCoroutines();
        //    this.StartCoroutine(KnockbackRoutine(collision.Attacker.Actor.forward));
        //}
        //private IEnumerator KnockbackRoutine(Vector3 direction)
        //{
        //    var knockbackDuration = 1.0f;
        //    var knockbackStrength = 10f;
        //    float timer = 0f;
        //    float volume = Mathf.PI * Mathf.Pow(characterController.radius, 2) * characterController.height * 2f;
        //    Vector3 velocity = direction.normalized * knockbackStrength / volume;
        //    while (timer < knockbackDuration)
        //    {
        //        characterController.Move(velocity * Time.deltaTime);

        //        timer += Time.deltaTime;
        //        yield return null;
        //    }
        //}
    }
}
