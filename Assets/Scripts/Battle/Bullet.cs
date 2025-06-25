using System;
using UnityEngine;
using Util;

namespace Battle
{
    public interface IBullet
    {
        public void OnFire();
    }
    public class Bullet : IBullet
    {
        private readonly AttackBox _attackBox;
        private readonly Transform _gun;
        private readonly Transform _actor;
        public float MaxDistance = 100f;
        public event Action<HitBoxCollision> OnHit;

        public Bullet(Transform gun, Transform actor)
        {
            _gun = gun;
            _actor = actor;
            _attackBox = new AttackBox(_actor, 0.1f);
            _attackBox.SetType(AttackBox.AttackType.None);
            _attackBox.OnCollision += DrawHitLine;
            _attackBox.OnCollision += (c) => OnHit?.Invoke(c);
        }
        public void OnFire()
        {
            Debug.Log(nameof(OnFire));
            var hits = Physics.RaycastAll(GetAim());
            _attackBox.OpenAttackWindow();
            Enumerator.InvokeFor(hits, DoHit);
            DrawBulletLine();
        }
        private void DoHit(RaycastHit hit)
        {
            if (!hit.transform.TryGetComponent<IHitBox>(out var victim))
            {
                return;
            }

            _attackBox.CheckCollision(new HitBoxCollision() { Attacker = _attackBox, Victim = victim.HitBox, HitPoint = hit.point });
        }
        void DrawHitLine(HitBoxCollision collision)
        {
            var ray = GetAim();
            Debug.DrawRay(ray.origin, collision.HitPoint - ray.origin, Color.red, 1f);
        }
        void DrawBulletLine()
        {
            var ray = GetAim();
            Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 1f);
        }
        private Ray GetAim()
        {
            return new Ray(Camera.main.transform.position, Camera.main.transform.forward * MaxDistance);
        }
    }
}
