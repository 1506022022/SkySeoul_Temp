using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Input1
{
    public class Joycon
    {
        private int currentFrame;
        private Action action;
        private readonly Dictionary<KeyCode, Action<float>> axisCommandMap;
        private readonly Dictionary<HashSet<KeyCode>, Action> keyCommandMap;
        private readonly Dictionary<int, int> actionPriority;
        private IInputProvider input;

        public Joycon()
        {
            axisCommandMap = new();
            keyCommandMap = new();
        }
        public void SetInputProvider(IInputProvider input)
        {
            this.input = input;
        }
        public void AddAction(HashSet<KeyCode> command, Action act)
        {
            if (keyCommandMap.TryAdd(command, act)) actionPriority.Add(act.GetHashCode(), actionPriority.Count);
            else Debug.LogWarning(DM_WARNING.ALREADY);
        }
        public void AddAction(KeyCode command, Action<float> act)
        {
            if (axisCommandMap.TryAdd(command, act)) actionPriority.Add(act.GetHashCode(), actionPriority.Count);
            else Debug.LogWarning(DM_WARNING.ALREADY);
        }

        private void FlashInput()
        {
            int maxPriority = int.MaxValue;
            foreach (var element in keyCommandMap)
            {
                if (!element.Key.All(k => input.GetKeyDown(k))) continue;
                if (!actionPriority.TryGetValue(element.Value.GetHashCode(), out var priority))
                {
                    priority = int.MaxValue;
                };
                action = priority > maxPriority ? action : element.Value;
            }
            foreach (var element in axisCommandMap)
            {
                var f = input.GetAxis(element.Key);
                if (f == 0) continue;
                if (!actionPriority.TryGetValue(element.Value.GetHashCode(), out var priority))
                {
                    priority = int.MaxValue;
                };
                action = priority > maxPriority ? action : () => element.Value.Invoke(f);
            }
        }
        private void Update()
        {
            if (currentFrame < Time.frameCount)
            {
                action?.Invoke();
                action = null;
                currentFrame = Time.frameCount;
                FlashInput();
            }
        }
    }

}
