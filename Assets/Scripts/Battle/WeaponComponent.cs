using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Battle
{
    public class WeaponComponent : MonoBehaviour
    {
        private Weapon _weapon;
        private Bullet _bullet;
        [Header("Animation")]
        [SerializeField] AnimationClip _attackClip;
        [SerializeField, Range(0, 1f)] private float _invokeTiming;
        [SerializeField] private UnityEvent _onFire;

        [Header("Effect")]
        [SerializeField] ParticleSystem _attackEffect;

        public void SetOwner(Character character, Transform actor)
        {
            _weapon = new Weapon(character);
            _weapon.OnFire += DoAttack;
            _bullet = new Bullet(gun: transform, actor);
            _onFire.AddListener(_bullet.OnFire);
            _bullet.OnHit += OnHit;
        }
        private void DoAttack()
        {
            var delay = 0f;
            if (_attackClip == null)
            {
                delay = 0;
            }
            else
            {
                delay = _attackClip.length * _invokeTiming;
            }
            StartCoroutine(DelayAttack(delay));
        }
        private IEnumerator DelayAttack(float delay)
        {
            yield return new WaitForSeconds(delay);
            _onFire.Invoke();
        }
        private void OnHit(HitBoxCollision collision)
        {
            var particle = ParticlePlayer.GetPool(_attackEffect).Get();
            particle.transform.position = collision.HitPoint;
            particle.Play();
            StartCoroutine(DelayRelease(() => ParticlePlayer.GetPool(_attackEffect).Release(particle)));
        }
        private IEnumerator DelayRelease(UnityAction action)
        {
            yield return new WaitForSeconds(1f);
            action.Invoke();
        }
    }
}
