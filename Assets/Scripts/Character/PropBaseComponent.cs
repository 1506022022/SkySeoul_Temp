using Battle;
using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public abstract class PropBaseComponent : EntityBaseComponent, IDamageable, IDeathable, IInitializable, IProp, IHitStun
    {
        [SerializeField] protected Animator animator;
        [SerializeField] protected HitBoxComponent body;
        HitBox IDamageable.HitBox { get => body?.HitBox ?? HitBox.Empty; }

        [SerializeField] bool initOnAwake;
        public bool IsDead { get; private set; }

        float IDeathable.DeathDuration { get; set; }

        float hitTime;
        bool IHitStun.IsHit => Time.time < hitTime;

        void IInitializable.Initialize()
        {
            OnInitialize();
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
            if (this is IHitStun hitStun && !hitStun.IsHit) hitStun.HitStun(0.1f);
            OnTakeDamage();
        }
        protected virtual void OnTakeDamage() { }
        private void OnEnable()
        {
            (this as IInitializable)?.Initialize();
        }

        private void Awake()
        {
            if (initOnAwake) (this as IInitializable).Initialize();
        }

        void IHitStun.HitStun(float hitDuration)
        {
            hitTime = Time.time + hitDuration;
        }
    }
}