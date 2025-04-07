using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public interface IInteraction
    {
        public Bounds Range { get; }
        public event Action OnStart;
        public event Action OnFail;
        public event Action OnSuccess;
        public void DoStart();
        public void DoFail();
        public void DoSuccess();
    }

    public static class InteractionSystem
    {
        private readonly static List<IInteraction> _interactions = new();

        public static bool TryGetInteraction(Transform actor, out IInteraction interaction)
        {
            interaction = null;
            foreach (var item in _interactions)
            {
                if (!item.Range.Contains(actor.position))
                {
                    continue;
                }
                interaction = item;
                return true;
            }
            return false;
        }
        public static void AddInteraction(IInteraction interaction)
        {
            _interactions.Add(interaction);
        }
        public static void RemoveInteraction(IInteraction interaction)
        {
            _interactions.Remove(interaction);
        }
    }

}
