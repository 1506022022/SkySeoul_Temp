using Character;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battle
{
    public class BattleController : IController
    {
        public event Action<IActor> OnDead;
        readonly BattleHUD battleHUD = new();
        readonly HashSet<IActor> joinCharacters = new();

        public void Update()
        {
        }
        public void Clear()
        {
            while (joinCharacters.Count > 0)
            {
                DisposeCharacter(joinCharacters.First());
            }
            battleHUD.Dispose();
        }
        public void JoinCharacter(IActor actor)
        {
            if (actor is IDamageable body)
                body.HitBox.OnCollision += OnHitCharacter;
            joinCharacters.Add(actor);
        }
        public void DisposeCharacter(IActor actor)
        {
            if (actor is IDamageable body)
                body.HitBox.OnCollision -= OnHitCharacter;
            joinCharacters.Remove(actor);
        }
        void OnHitCharacter(HitBoxCollision collision)
        {
            if (!collision.Victim.Actor.TryGetComponent<IActor>(out var actor)) return;
            if (actor is IDeathable death && death.IsDead) return;
            if (actor is IHP health)
            {
                Action<IHP> updateHUD = health switch
                {
                    IPlayable => battleHUD.UpdatePlayer,
                    _ => battleHUD.UpdateMonster
                };
                updateHUD.Invoke(health);
                health.HP.Value--;
                updateHUD.Invoke(health);
                if (health.HP.Value <= 0) DoDie(actor);
                else if (actor is IDamageable damageable) damageable.TakeDamage();
            }


            else if (actor is IDamageable damageable) damageable.TakeDamage();
        }
        void DoDie(IActor actor)
        {
            this.DisposeCharacter(actor);
            if (actor is IDeathable death) death.Die();
            OnDead?.Invoke(actor);
        }
    }
}
