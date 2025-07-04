using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    public class GunComponent : WeaponComponent
    {
        public float maxDistance;
        Bullet bullet;
        AttackData attackData;
        [Header("Animation")]
        [SerializeField] private UnityEvent onFire;

        protected override void Initialize(CharacterState character, Transform actor)
        {
            base.Initialize(character, actor);
            bullet = new Bullet(transform, actor);
        }
        protected override void DoAttack()
        {
            StartCoroutine(DelayAction(bullet.OnFire, attackData.PreDelay));
        }
        private IEnumerator DelayAction(UnityAction action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
            onFire.Invoke();
        }
    }
}
