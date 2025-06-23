using System;
using System.Collections.Generic;

namespace Battle
{
    public class BattleController : IController
    {
        public event Action<CharacterComponent> OnDead;
        readonly BattleHUD battleHUD = new();
        readonly List<CharacterComponent> joinCharacters = new();

        public void Update()
        {
        }
        public void Clear()
        {
            while (joinCharacters.Count > 0)
            {
                DisposeCharacter(joinCharacters[0]);
            }
        }
        public void JoinCharacter(CharacterComponent character)
        {
            character.Body.HitBox.OnCollision += OnHitCharacter;
        }
        public void DisposeCharacter(CharacterComponent character)
        {
            character.Body.HitBox.OnCollision -= OnHitCharacter;
            joinCharacters.Remove(character);
        }
        void OnHitCharacter(HitBoxCollision collision)
        {
            if (!collision.Victim.Actor.TryGetComponent<CharacterComponent>(out var character)) return;

            Action<CharacterComponent> updateHUD = character switch
            {
                IPlayable => battleHUD.UpdatePlayer,
                _ => battleHUD.UpdateMonster
            };
            updateHUD.Invoke(character);
            character.HP.Value--;
            updateHUD.Invoke(character);

            if (character.HP.Value > 0) character.DoHit(); else DoDie(character);
        }
        void DoDie(CharacterComponent character)
        {
            this.DisposeCharacter(character);
            character.DoDie();
            OnDead?.Invoke(character);
        }
    }
}
