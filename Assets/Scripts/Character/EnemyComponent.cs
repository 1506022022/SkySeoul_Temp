using Battle;
using UnityEngine;

namespace Character
{
    public class EnemyComponent : CharacterBaseComponent, IEnemy, ITraveler, IStun, IHitStun
    {
        [SerializeField] AttackBoxComponent weapon;

        float stunTime;
        bool IStun.IsStun => Time.time < stunTime;
        void IStun.Stun(float stunDuration) => stunTime = Time.time;
        float hitTime;
        bool IHitStun.IsHit => Time.time < hitTime;
        void IHitStun.HitStun(float hitDuration) => hitTime = Time.time;

        void ITraveler.StartTravel()
        {
            animator.SetBool("IsMove", true);
            animator.SetFloat("MoveSpeed", 1.0f);
            OnStartTravel();
        }
        protected virtual void OnStartTravel() { }

        void ITraveler.EndTravel()
        {
            animator.SetBool("IsMove", false);
            animator.SetFloat("MoveSpeed", 0f);
            OnEndTravel();
        }
        protected virtual void OnEndTravel() { }
        protected override void OnAttack(int attackType)
        {
            base.OnAttack(attackType);
            if (weapon != null) weapon.AttackBox.OpenAttackWindow();
        }
    }
}