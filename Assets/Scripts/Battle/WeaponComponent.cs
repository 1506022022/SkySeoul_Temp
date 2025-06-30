using UnityEngine;

namespace Battle
{
    public abstract class WeaponComponent : MonoBehaviour
    {
        Transform actor;
        Weapon weapon;
        public void SetOwner(CharacterState character, Transform actor)
        {
            this.actor = actor;
            weapon = new Weapon(character);
            weapon.OnFire += DoAttack;

            Initialize(character, actor);
        }
        protected abstract void DoAttack();
        protected virtual void Initialize(CharacterState character, Transform actor) { }
    }
}
