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

        protected override void Initialize(Character character, Transform actor)
        {
            base.Initialize(character, actor);
            bullet = new Bullet(transform, actor);
            onFire.AddListener(bullet.OnFire);
        }
        protected override void DoAttack()
        {
            StartCoroutine(DelayAction(onFire.Invoke, attackData.PreDelay));
        }
        private IEnumerator DelayAction(UnityAction action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
