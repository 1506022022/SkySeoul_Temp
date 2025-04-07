using System;
using UnityEngine;

namespace Battle
{
    public class CollisionBox
    {
        public Transform Actor
        {
            get; private set;
        }
        public event Action<HitBoxCollision> OnCollision;

        public CollisionBox(Transform actor)
        {
            Actor = actor;
        }
        protected static void InvokeCollision(HitBoxCollision collision)
        {
            collision.Attacker.InvokeEvent(collision);
            collision.Victim.InvokeEvent(collision);
        }
        private void InvokeEvent(HitBoxCollision collision)
        {
            OnCollision?.Invoke(collision);
        }
    }

}
