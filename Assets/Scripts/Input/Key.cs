using System.Collections.Generic;
using UnityEngine;

namespace Input1
{
    public interface IInputProvider
    {
        public float GetAxis(KeyCode key);
        public bool GetKey(KeyCode key);
        public bool GetKeyDown(KeyCode key);
        public bool GetKeyUp(KeyCode key);
    }
    public class PlayerInputProvider : IInputProvider
    {
        public float GetAxis(KeyCode key) => Input.GetAxis(key.ToString());
        public bool GetKey(KeyCode key) => Input.GetButton(key.ToString());
        public bool GetKeyDown(KeyCode key) => Input.GetButtonDown(key.ToString());
        public bool GetKeyUp(KeyCode key) => Input.GetButtonUp(key.ToString());
    }
    public class LockedInputProvider : IInputProvider
    {
        public float GetAxis(KeyCode key) => 0f;
        public bool GetKey(KeyCode key) => false;
        public bool GetKeyDown(KeyCode key) => false;
        public bool GetKeyUp(KeyCode key) => false;
    }
    public class ControlInputProvider : IInputProvider
    {
        private struct InputTime
        {
            public float StartTime;
            public float EndTime;
        }
        private readonly Dictionary<KeyCode, InputTime> keyMap;
        private readonly Dictionary<KeyCode, float> axisMap;

        public ControlInputProvider()
        {
            keyMap = new();
            axisMap = new();
        }
        public float GetAxis(KeyCode key)
        {
            if (!axisMap.ContainsKey(key)) return 0f;
            return axisMap[key];
        }
        public bool GetKey(KeyCode key)
        {
            if (!keyMap.ContainsKey(key)) return false;
            return keyMap[key].StartTime < Time.frameCount && keyMap[key].EndTime <= Time.frameCount;
        }
        public bool GetKeyDown(KeyCode key)
        {
            if (!keyMap.ContainsKey(key)) return false;
            return keyMap[key].StartTime == Time.frameCount;
        }
        public bool GetKeyUp(KeyCode key)
        {
            if (!keyMap.ContainsKey(key)) return false;
            return keyMap[key].EndTime + 1 == Time.frameCount;
        }
        public void SetAxisValue(KeyCode key, float value)
        {
            axisMap.TryAdd(key, value);
            axisMap[key] = value;
        }
        public void SetKeyValue(KeyCode key)
        {
            if (keyMap.TryGetValue(key, out var inputTime))
            {
                inputTime.EndTime = Time.frameCount;
            }
            else
            {
                inputTime = new InputTime
                {
                    StartTime = Time.frameCount,
                    EndTime = Time.frameCount
                };
            }

            keyMap[key] = inputTime;
        }
    }
}