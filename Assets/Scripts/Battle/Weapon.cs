using System;

namespace Battle
{
    public class Weapon
    {
        private readonly Character _character;
        public event Action OnFire;

        public Weapon(Character character)
        {
            _character = character;
            _character.OnAttack += OnAttack;
        }

        private void OnAttack()
        {
            OnFire?.Invoke();
        }

    }

}
