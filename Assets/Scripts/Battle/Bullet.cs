using System;
using UnityEngine;
using Util;

namespace Battle
{
    public class Bullet
    {
        private readonly AttackBox _attackBox;
        private readonly Transform _gun;
        private readonly Transform _actor;
        private float _maxDistance = 100f;
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
            var hits = Physics.RaycastAll(_gun.position, GetShootDir(), _maxDistance);
            _attackBox.OpenAttackWindow();
            Enurmerator.InvokeFor(hits, DoHit);
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
        private void DrawHitLine(HitBoxCollision collision)
        {
            Debug.DrawRay(_gun.transform.position, collision.HitPoint - _gun.transform.position, Color.red, 1f);
        }
        private void DrawBulletLine()
        {
            Debug.DrawRay(_gun.transform.position, GetShootDir() * _maxDistance, Color.yellow, 1f);
        }
        private Vector3 GetShootDir()
        {
            Vector3 gunPosition = _gun.position;
            Vector3 screenCenter = new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane);
            Vector3 worldCenter = Camera.main.ScreenToWorldPoint(screenCenter);
            Vector3 directionToCenter = gunPosition - worldCenter;
            return directionToCenter.normalized;
        }
    }
}
