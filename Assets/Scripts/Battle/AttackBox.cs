using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Battle
{
    public class AttackBox : CollisionBox
    {
        public enum AttackType { None, OneHit };
        private readonly Delay _delay;
        private readonly List<HitBox> _attacked = new();
        private bool _bNotWithinAttackWindow => !_delay.IsDelay() || (_attackType == AttackType.OneHit && _bHit);
        private bool _bHit;
        private AttackType _attackType;

        public AttackBox(Transform actor, float attackWindow = 0f) : base(actor)
        {
            _delay = new Delay(attackWindow);
            _delay.StartTime = -1f;
        }

        public void CheckCollision(HitBoxCollision collision)
        {
            if (_bNotWithinAttackWindow)
            {
                return;
            }

            if (_bNotWithinAttackWindow ||
                collision.Victim.Actor.Equals(Actor) ||
                _attacked.Contains(collision.Victim))
            {
                return;
            }

            _attacked.Add(collision.Victim);
            CollisionBox.InvokeCollision(collision);
            _bHit = true;
        }

        public void OpenAttackWindow()
        {
            _bHit = false;
            _delay.DoStart();
            _attacked.Clear();
        }

        public void SetType(AttackType type)
        {
            _attackType = type;
        }

    }
}