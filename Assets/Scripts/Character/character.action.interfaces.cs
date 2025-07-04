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
    public interface IDeathSkillOwner
    {
        public SkillComponent DeathSkill { get; set; }
        public Vector3 DeathSkillOffset { get; set; }
        public Vector3 DeathSkillRotation { get; set; }
    }
}

