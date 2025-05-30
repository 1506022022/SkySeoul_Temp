using System;
using UnityEngine;
using UnityEngine.Events;

namespace BattlePath
{
    public abstract class EventPathBase
    {
        public abstract void DrawGizmos();
        public abstract void InvokeEvent();
    }


    [Serializable]
    public class EventPath : EventPathBase
    {
        public float t;
        public AnimationPath3 _path;
        public Color _eventType = Color.yellow;
        [SerializeField] private ScriptableObject _eventObject;
        [SerializeField] private UnityEvent _Testevent;

        public override void DrawGizmos()
        {
            if (_path == null)
            {
                return;
            }
            Gizmos.color = _eventType;
            Gizmos.DrawWireCube(_path.Evaluate(t), Vector3.one);
        }

        public override void InvokeEvent()
        {
            Debug.Log($"Invoke Event!! Time : {t} ");
            _Testevent.Invoke();
        }
    }
}
