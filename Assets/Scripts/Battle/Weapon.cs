using System;

namespace Battle
{
    public class Weapon
    {
        private readonly CharacterState _character;
        public event Action OnFire;

        public Weapon(CharacterState character)
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
