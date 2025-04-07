using System;
using UnityEngine;

namespace Battle
{
    public class Interaction : IInteraction, IDestroy
    {
        public event Action OnStart;
        public event Action OnFail;
        public event Action OnSuccess;
        public Bounds Range { get; private set; }

        public Interaction(Bounds range)
        {
            SetRange(range);
            InteractionSystem.AddInteraction(this);
        }
        public void SetRange(Bounds range)
        {
            Range = range;
        }
        public void OnDestroy()
        {
            InteractionSystem.RemoveInteraction(this);
        }
        public void DoStart()
        {
            OnStart?.Invoke();
        }
        public void DoFail()
        {
            OnFail?.Invoke();
        }
        public void DoSuccess()
        {
            OnSuccess?.Invoke();
        }
    }
}
