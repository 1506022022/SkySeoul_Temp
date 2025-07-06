using Battle;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement;

namespace Character
{
    public abstract class CharacterBaseComponent : EntityBaseComponent, IDamageable, IDeathable, IMovable, IInitializable, IAttackable, IGroundCheckable, IHP, IDisposable
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected HitBoxComponent body;
        Statistics IHP.HP { get; } = new(1);
        HitBox IDamageable.HitBox { get => body?.HitBox ?? HitBox.Empty; }
        readonly IMove walk = new Walk();
        readonly IMove gravity = new ReciveGravity();
        public bool IsGrounded => Physics.Raycast(transform.position, Vector3.down, 0.3f, LayerMask.GetMask("Ground"));
        public bool IsDead { get; private set; } = true;
        float IDeathable.DeathDuration { get; set; }
        [SerializeField] bool initOnEnable;
        void IInitializable.Initialize()
        {
            (this as IDeathable).Revive();
            walk.SetActor(transform);
            gravity.SetActor(transform);
            OnInitialize();
        }
        protected virtual void OnInitialize() { }
        void IDisposable.Dispose()
        {
            (this as IDeathable)?.Die();
        }
        protected virtual void OnDispose() { }
        void IAttackable.Attack(int attackType)
        {
            animator.SetTrigger("Attack");
            animator.SetInteger("AttackType", attackType);
            OnAttack(attackType);
        }
        protected virtual void OnAttack(int attackType) { }
        void IDeathable.Die()
        {
            IsDead = true;
            animator.SetBool("IsDead", IsDead);
            OnDie();
        }
        protected virtual void OnDie() { }

        void IMovable.Move(Vector3 direction, float strength)
        {
            direction.Normalize();
            animator.SetBool("IsMove", true);
            (walk as IStrength)?.SetStrength(strength);
            (walk as IDirection)?.SetDirection(direction);
            OnMove(direction, strength);
        }
        protected virtual void OnMove(Vector3 direction, float strength) { }
        void IDeathable.Revive()
        {
            IsDead = false;
            animator.SetBool("IsDead", IsDead);
            OnRevive();
        }
        protected virtual void OnRevive() { }
        void IDamageable.TakeDamage()
        {
            animator.SetTrigger("Damaged");
            OnTakeDamage();
        }
        protected virtual void OnTakeDamage() { }

        protected virtual void OnEnable()
        {
            if (initOnEnable)
                (this as IInitializable).Initialize();
        }

        protected void LateUpdate()
        {
            OnLateUpdate();
        }
        protected virtual void OnLateUpdate() { }
        protected void FixedUpdate()
        {
            (walk as IUpdateReceiver).Update(Time.fixedDeltaTime);
            (gravity as IUpdateReceiver).Update(Time.fixedDeltaTime);

            if (walk is IStrength walkStrength && walkStrength.GetStrength() != 0)
            {
                var strength = walkStrength.GetStrength();
                if (0.5f < strength)
                {
                    animator.SetFloat("MoveSpeed", strength);
                }
                else
                {
                    animator.SetBool("IsMove", false);
                    walkStrength.SetStrength(0f);
                }
            }

            OnFixedUpdate();
        }
        protected virtual void OnFixedUpdate() { }
    }
}