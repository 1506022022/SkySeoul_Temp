using Character;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public abstract class CharacterComponent : MonoBehaviour, IInitializable, IDisposable, IActor, ITransform, IDamageable, IDeathable, IHP
    {
        protected CharacterState character { get; private set; }
        IController controller;
        public bool IsGrounded => characterMovement.IsGrounded;
        CharacterAnimator characterAnimator;
        IMovement characterMovement;
        internal BodyState State => character.BodyState;
        [SerializeField] Animator animator;

        [Header("Battle")]
        public float MoveSpeed = 3f;
        public float JumpPower = 5f;

        public float deathDuration;
        [SerializeField] WeaponComponent weapon;
        [field: SerializeField] public HitBoxComponent Body { get; private set; }

        HitBox IDamageable.HitBox { get => Body?.HitBox ?? HitBox.Empty; }

        public Statistics HP { get; } = new Statistics(1);

        bool IDeathable.IsDead => throw new NotImplementedException();

        float IDeathable.DeathDuration { get; set; }

        [Header("Debug")]
        [SerializeField] TextMeshProUGUI ui;

        public virtual void Initialize()
        {
            character = new();
            character.OnAttack += OnAttack;
            character.OnMove += OnMove;
            character.OnRun += OnRun;
            character.OnJump += OnJump;
            character.OnLand += OnLand;
            character.OnHit += OnHit;
            character.OnCancel += OnCancel;
            character.OnInteraction += OnInteraction;
            character.OnSlide += OnSlide;
            character.OnDead += OnDie;

            weapon?.SetOwner(character, actor: transform);
            HP.Value = HP.MaxValue;
        }
        public virtual void Dispose()
        {
            SetAnimator(new EmptyAnimator());
            SetController(new EmptyJoycon(this as IActor));
            SetMovement(new EmptyMovement());
        }
        public void SetAnimator(CharacterAnimator characterAnimator)
        {
            if (characterAnimator == this.characterAnimator) return;
            this.characterAnimator?.Unuse();
            this.characterAnimator = characterAnimator;
            this.characterAnimator.Initialize(character, animator);
            this.characterAnimator.Use();
        }
        public void SetController(IController controller)
        {
            this.controller = controller;
        }
        public void SetMovement(IMovement movement)
        {
            if (movement == characterMovement) return;
            characterMovement = movement;
        }
        public void SetTeam(int team)
        {
            gameObject.layer = team;
            if (Body != null) Body.gameObject.layer = team;
            if (weapon != null) weapon.gameObject.layer = team;
        }
        public void DoAttack()
        {
            character.DoAttack();
        }
        protected virtual void OnAttack()
        {

        }
        public void DoDie()
        {
            character.DoDie();
        }
        protected virtual void OnDie()
        {
        }
        public void DoHit()
        {
            character.DoHit();
        }
        protected virtual void OnHit()
        {
        }
        public void DoInteraction()
        {
            character.DoInteraction();
        }
        protected virtual void OnInteraction()
        {
            if (InteractionSystem.TryGetInteraction(transform, out var interaction))
            {
                interaction.DoStart();
            }
        }
        public void DoMove(Vector3 position)
        {
            character.DoMove(position);
        }
        protected virtual void OnMove(Vector3 position)
        {

        }
        public void DoRun(Vector3 position)
        {
            character.DoRun(position);
        }
        protected virtual void OnRun(Vector3 position)
        {

        }
        public void DoSlide()
        {
            character.DoSlide();
        }
        protected virtual void OnSlide()
        {
        }
        public void DoJump()
        {
            character.DoJump();
        }
        protected virtual void OnJump()
        {
        }
        public void DoLand()
        {
            character.DoLand();
        }
        protected virtual void OnLand()
        {
        }
        public void DoCancel()
        {
            character.DoCancel();
        }
        protected virtual void OnCancel()
        {

        }
        protected virtual void Awake()
        {
            Initialize();
        }
        protected virtual void Update()
        {
            controller?.Update();
        }
        protected virtual void FixedUpdate()
        {
            characterMovement.UpdateGravity();
            ui?.SetText(character.BodyState.ToString());
        }
#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (characterMovement is CharacterMovement move)
            {
                move.JumpPower = JumpPower;
                move.MoveSpeed = MoveSpeed;
            }
        }
#endif
        protected virtual void OnDestroy()
        {
            characterAnimator?.Unuse();
        }

        void IDamageable.TakeDamage()
        {
            DoHit();
        }

        void IDeathable.Revive()
        {
        }

        void IDeathable.Die()
        {
            DoDie();
        }
    }
}