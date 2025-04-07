using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public class Lerp
    {
        private float _t;
        private string _name;
        private float _value;
        private float _preValue;
        private float _duration = 1f;
        private Animator _animator;
        private Coroutine _coroutine;
        private static readonly CoroutineRunner _runner = CoroutineRunner.instance;

        public Lerp SetAnimator(Animator animator)
        {
            _animator = animator;
            return this;
        }
        public Lerp SetFloat(string name, float value)
        {
            _t = 0f;
            _name = name;
            _value = value;
            _preValue = _animator.GetFloat(name);

            if (_coroutine != null)
            {
                _runner.StopCoroutine(_coroutine);
            }
            _coroutine = _runner.StartCoroutine(UpdateLerp());
            return this;
        }
        /// <summary>
        /// duration is greater than or equal to 0.001
        /// </summary>
        /// <param name="duration"></param>
        public Lerp SetDuration(float duration)
        {
            _duration = Mathf.Max(duration,0f);
            return this;
        }
        private IEnumerator UpdateLerp()
        {
            if (_animator == null)
            {
                yield break;
            }

            if (_duration == 0f)
            {
                _animator.SetFloat(_name, _value);
                yield break;
            }

            while (_t <= 1)
            {
                yield return null;
                _t = Mathf.Clamp01(_t + (Time.deltaTime / _duration));
                _animator.SetFloat(_name, Mathf.Lerp(_preValue, _value, _t));
            }
        }
    }
}
