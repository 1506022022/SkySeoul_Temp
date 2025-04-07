using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattlePath
{
    public abstract class AnimationPathBase
    {
        public abstract Vector3 Evaluate(float t);
    }

    public class AnimationCurveRenderer
    {
        public float maxTime = 1f;
        public int resolution = 100;
        public Color curveColor = Color.green;
        public Vector3 pivot = Vector3.zero;
        private readonly AnimationPath3 _path;
        private float _limitDraw = 1000;

        public AnimationCurveRenderer(AnimationPath3 path)
        {
            _path = path;
        }
        public void DrawCurve()
        {
            if (_path == null || resolution < 2)
                return;

            Vector3 prevPoint = pivot + _path.Evaluate(0);
            var r = Mathf.Min(resolution * maxTime, _limitDraw);
            for (int i = 1; i <= r; i++)
            {
                float t = (i / r) * maxTime;
                Vector3 newPoint = pivot + _path.Evaluate(t);
                Debug.DrawLine(prevPoint, newPoint, curveColor);
                prevPoint = newPoint;
            }
        }
    }

    public class AnimationPath3 : AnimationPathBase
    {
        private readonly AnimationCurve _x;
        private readonly AnimationCurve _y;
        private readonly AnimationCurve _z;
        public Vector3 Multiply = Vector3.one;

        public AnimationPath3(AnimationCurve x, AnimationCurve y, AnimationCurve z)
        {
            _x = x;
            _y = y;
            _z = z;
        }
        public override Vector3 Evaluate(float t)
        {
            var x = _x.Evaluate(t);
            var y = _y.Evaluate(t);
            var z = _z.Evaluate(t);
            return Vector3.Scale(new Vector3(x, y, z), Multiply);
        }
    }

    public abstract class AnimationPathComponent : MonoBehaviour
    {
        private AnimationCurveRenderer _renderer;
        public AnimationCurveRenderer Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = new(_path);
                }
                return _renderer;
            }
        }
        private AnimationPath3 _path;
        public AnimationPath3 Path
        {
            get
            {
                if (_path == null)
                {
                    _path = new(_x, _y, _z);
                }
                return _path;
            }
        }
        public List<EventPath> _events = new();
        [SerializeField] private AnimationCurve _x = new();
        [SerializeField] private AnimationCurve _y = new();
        [SerializeField] private AnimationCurve _z = new();
        [SerializeField] private Vector3 _multiply = Vector3.one;
        [SerializeField, Range(1, 3600f)] private float _maxTime;

        private float _preT;

        [field: SerializeField, Range(0, 3600f)]
        public float T
        {
            get; set;
        }
        protected abstract void OnUpdatePath(Vector3 position);
        protected abstract void OnUpdateTime(float normalizeTime);
        private void OnUpdateTime(float oldTime, float newTime)
        {
            foreach (var e in _events)
            {
                if (e.t < oldTime)
                {
                    continue;
                }

                if (e.t > newTime)
                {
                    break;
                }

                e.InvokeEvent();
            }
            OnUpdateTime(newTime / _maxTime);
            OnUpdatePath(Path.Evaluate(newTime));
        }
        private void Awake()
        {
            _events = _events.OrderBy(x => x.t).ToList();
        }
        private void OnDrawGizmosSelected()
        {
            Path.Multiply = _multiply;
            Renderer.DrawCurve();
            Renderer.maxTime = _maxTime;
            foreach (var e in _events)
            {
                if (e == null)
                {
                    return;
                }
                if (e._path == null)
                {
                    e._path = _path;
                }
                e.t = Mathf.Clamp(e.t, 0, _maxTime);
                e.DrawGizmos();
            }
            if (_preT != T)
            {
                T = Mathf.Clamp(T, 0, _maxTime);
                OnUpdateTime(_preT, T);
                _preT = T;
            }
        }
    }

}
