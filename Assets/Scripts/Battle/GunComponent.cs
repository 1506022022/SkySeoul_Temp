using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Util;

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
        }
        protected override void DoAttack()
        {
            Debug.Log(nameof(DoAttack));
            StartCoroutine(DelayAction(bullet.OnFire, attackData.PreDelay));
        }
        private IEnumerator DelayAction(UnityAction action, float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log(nameof(DelayAction));
            action?.Invoke();
            onFire.Invoke();
        }
    }
}
