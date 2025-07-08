using Battle;
using UnityEngine;

namespace Character
{
    public interface IGroundCheckable { bool IsGrounded { get; } }
    public interface IJumpable { void Jump(); }
    public interface IAttackable { void Attack(int attackType); }
    public interface IDamageable { void TakeDamage(); HitBox HitBox { get; } }
    public interface IDeathable { void Revive(); void Die(); bool IsDead { get; } float DeathDuration { get; set; } }
    public interface IMovable { void Move(Vector3 direction, float strength); }
    public interface ISliding { void Slide(); }
    public interface IHP { Statistics HP { get; } }
    public interface IControlable { void SetController(IController controller); }
    public interface ISkillOwner
    {
        public SkillComponent Skill { get; set; }
        public Vector3 SkillOffset { get; set; }
        public Vector3 SkillRotation { get; set; }
    }
    public interface ITraveler
    {
        void StartTravel();
        void EndTravel();
    }
    public interface IHitStun
    {
        void HitStun(float hitDuration);
        bool IsHit { get; }
    }
    public interface IStun
    {
        void Stun(float stunDuration);
        bool IsStun { get; }
    }
    public interface ICaster
    {
        void Invoke();
        void Cancel();
    }
}