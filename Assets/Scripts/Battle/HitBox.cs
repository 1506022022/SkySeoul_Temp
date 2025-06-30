using System;
using UnityEngine;

namespace Battle
{
    public struct HitBoxCollision
    {
        public HitBox Victim;
        public AttackBox Attacker;
        public Vector3 HitPoint;
    }

    public interface IHitBox
    {
        public HitBox HitBox { get; }
    }

    [Serializable]
    public class HitBox : CollisionBox
    {
        public static readonly HitBox Empty = new HitBox(null);

        public HitBox(Transform actor) : base(actor)
        {
        }
    }
}
