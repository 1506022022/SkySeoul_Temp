using Battle;
using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public abstract class PropBaseComponent : EntityBaseComponent, IDamageable, IDeathable, IInitializable, IProp
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected HitBoxComponent body;
        HitBox IDamageable.HitBox { get => body?.HitBox ?? HitBox.Empty; }
        playercontroller controller;

        public bool IsDead { get; private set; }

        float IDeathable.DeathDuration { get; set; }

        void IInitializable.Initialize()
        {
            OnInitialize();
            controller = new(this);
        }
        protected virtual void OnInitialize() { }
        void IDeathable.Die()
        {
            IsDead = true;
            animator?.SetBool("IsDead", IsDead);
            OnDie();
        }
        protected virtual void OnDie() { }
        void IDeathable.Revive()
        {
            IsDead = false;
            animator?.SetBool("IsDead", IsDead);
            OnRevive();
        }
        protected virtual void OnRevive() { }
        void IDamageable.TakeDamage()
        {
            animator?.SetTrigger("Damaged");
            OnTakeDamage();
        }
        protected virtual void OnTakeDamage() { }
        private void OnEnable()
        {
            (this as IInitializable)?.Initialize();
        }
        private void LateUpdate()
        {
            controller?.Update();
        }
    }
}