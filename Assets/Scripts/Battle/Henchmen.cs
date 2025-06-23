using System;
using UnityEngine;

namespace Battle
{
    public enum Team : byte { None, Monster, NPC, Player }
    public class Henchmen : IController
    {
        CharacterComponent character;
        public static event Action<Henchmen> OnSpawnEvent;
        public static event Action<Henchmen> OnDestroyEvent;
        public Team Team;
        public bool HasDependency { get; set; }
        public Vector3 Position => character.transform.position;

        public Henchmen(CharacterComponent character)
        {
            this.character = character;
        }
        Vector3 prePosition;
        Vector3 targetPosition;
        public void MoveTo(Vector3 position)
        {
            targetPosition = position;
        }

        public void Attack()
        {
            character.DoAttack();
        }

        bool FindPlayer(out Transform player)
        {
            player = GameObject.FindAnyObjectByType<ZoomCharacterComponent>()?.transform;
            return player != null;
        }
        public void Update()
        {
            PlayerControl.UpdateLand(character);

            if (FindPlayer(out var player) && Vector3.Distance(player.position, character.transform.position) < 1f)
            {
                character.transform.LookAt(player.position);
                Attack();
                return;
            }

            if (prePosition != targetPosition)
            {
                prePosition = targetPosition;
                character.DoMove(targetPosition);
            }
        }
    }
}
