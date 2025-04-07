using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    public abstract class CharacterAnimator
    {
        protected const string STATE_ATTACK = "Attack";
        protected const string STATE_IDLE = "Idle";
        protected const string STATE_JUMP = "Jump";
        protected const string STATE_LAND = "Land";
        protected const string STATE_HIT = "Hit";
        protected const string STATE_MOVE = "Move";
        protected const string STATE_CANCEL = "Cancel";
        protected const string STATE_FALL = "Fall";
        protected const string STATE_SLIDING = "Sliding";
        protected const string STATE_CROUCH = "Crouch";
        protected const string PARAMETERS_CALMDOWN = "CalmDown";
        protected const string PARAMETERS_SPEED = "Speed";
        protected const string PARAMETERS_CROUCH = "Crouch";
        protected const string PARAMETERS_INTERACTION = "Interaction";
        protected const string PARAMETERS_HORIZONTAL = "Horizontal";
        protected const string PARAMETERS_VERTICAL = "Vertical";
        protected readonly Animator _animator;
        protected readonly Character _character;
        protected readonly Dictionary<string, int> _stringToHash = new();
        protected readonly Dictionary<int, string> _hashToString = new();
        private readonly Dictionary<int, Lerp> _lerps = new();
        private readonly Dictionary<int, Coroutine> _awaits = new();
        private string _preTrigger = STATE_IDLE;
        private static readonly CoroutineRunner _runner = CoroutineRunner.instance;

        public CharacterAnimator(Character character, Animator animator)
        {
            _character = character;
            _animator = animator;

            _stringToHash.Add(STATE_IDLE, Animator.StringToHash(STATE_IDLE));
            _stringToHash.Add(STATE_MOVE, Animator.StringToHash(STATE_MOVE));
            _stringToHash.Add(STATE_ATTACK, Animator.StringToHash(STATE_ATTACK));
            _stringToHash.Add(STATE_JUMP, Animator.StringToHash(STATE_JUMP));
            _stringToHash.Add(STATE_LAND, Animator.StringToHash(STATE_LAND));
            _stringToHash.Add(STATE_HIT, Animator.StringToHash(STATE_HIT));
            _stringToHash.Add(STATE_CANCEL, Animator.StringToHash(STATE_CANCEL));
            _stringToHash.Add(STATE_FALL, Animator.StringToHash(STATE_FALL));
            _stringToHash.Add(STATE_SLIDING, Animator.StringToHash(STATE_SLIDING));
            _stringToHash.Add(PARAMETERS_VERTICAL, Animator.StringToHash(PARAMETERS_VERTICAL));
            _stringToHash.Add(PARAMETERS_HORIZONTAL, Animator.StringToHash(PARAMETERS_HORIZONTAL));
            _stringToHash.Add(PARAMETERS_SPEED, Animator.StringToHash(PARAMETERS_SPEED));
            _hashToString.AddRange(_stringToHash.ToDictionary(kv => kv.Value, kv => kv.Key));
        }
        
        public void Use()
        {
            RemoveCharacterEvent();
            AddCharacterEvnet();
            OnUse();
        }
        public void Unuse()
        {
            RemoveCharacterEvent();
            OnUnuse();
        }
        protected abstract void OnUse();
        protected abstract void OnUnuse();
        protected abstract void OnIdle();
        protected abstract void OnMove(Vector3 dir);
        protected abstract void OnRun(Vector3 dir);
        protected abstract void OnAttack();
        protected abstract void OnJump();
        protected abstract void OnFall();
        protected abstract void OnLand();
        protected abstract void OnSlide();
        protected abstract void OnSlideEnd();
        protected abstract void OnStandUp();
        protected abstract void OnCancel();
        protected abstract void OnInteraction();
        protected abstract void OnCalmDown();
        protected abstract void OnHit();
        protected void SetFloat(int parameter, float value, float duration = 0f)
        {
            GetLerp(parameter).SetDuration(duration)
                              .SetFloat(_hashToString[parameter], value);
        }
        protected void SetTrigger(string trigger, int layer)
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(layer);
            var state = stateInfo.shortNameHash;
            var loop = stateInfo.loop;
            if (!loop && _preTrigger == trigger && state == _stringToHash[trigger])
            {
                _animator.Play(state, layer, 0f);
            }
            else
            {
                _animator.ResetTrigger(_preTrigger);
                _animator.SetTrigger(trigger);
            }
            _preTrigger = trigger;
        }
        protected void SetBoolean(string boolean, bool value)
        {
            _animator.SetBool(boolean, value);
        }
        protected void AwaitExitEvent(string exitState, int layer, UnityAction exitEvent)
        {
            if (_awaits.TryGetValue(_stringToHash[exitState], out var await))
            {
                _runner.StopCoroutine(await);
            }

            _awaits[_stringToHash[exitState]] = _runner.StartCoroutine(InvokeEixitEvent(_stringToHash[exitState], layer, exitEvent));
        }
        private IEnumerator InvokeEixitEvent(int exitState, int layer, UnityAction exitEvent)
        {
            yield return new WaitUntil(() => IsSame(exitState, layer));
            yield return new WaitUntil(() => IsExit(exitState, layer));
            exitEvent?.Invoke();
        }
        private bool IsSame(int exitState, int layer)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(layer);
            return state.shortNameHash == exitState;
        }
        private bool IsExit(int exitState, int layer)
        {
            var state = _animator.GetCurrentAnimatorStateInfo(layer);
            var outState = state.shortNameHash != exitState;
            var outRange = 1 <= state.normalizedTime;
            return outState || outRange;
        }
        private Lerp GetLerp(int parameter)
        {
            if (!_lerps.TryGetValue(parameter, out var value))
            {
                value = new Lerp();
                value.SetAnimator(_animator);
                _lerps.Add(parameter, value);
            }
            return value;
        }
        private void AddCharacterEvnet()
        {
            _character.OnIdle += OnIdle;
            _character.OnMove += OnMove;
            _character.OnRun += OnRun;
            _character.OnAttack += OnAttack;
            _character.OnJump += OnJump;
            _character.OnFall += OnFall;
            _character.OnLand += OnLand;
            _character.OnSlide += OnSlide;
            _character.OnSlideEnd += OnSlideEnd;
            _character.OnStandup += OnStandUp;
            _character.OnCancel += OnCancel;
            _character.OnInteraction += OnInteraction;
            _character.OnCalmDown += OnCalmDown;
            _character.OnHit += OnHit;
        }
        private void RemoveCharacterEvent()
        {
            _character.OnIdle -= OnIdle;
            _character.OnMove -= OnMove;
            _character.OnRun -= OnRun;
            _character.OnAttack -= OnAttack;
            _character.OnJump -= OnJump;
            _character.OnFall -= OnFall;
            _character.OnLand -= OnLand;
            _character.OnSlide -= OnSlide;
            _character.OnSlideEnd -= OnSlideEnd;
            _character.OnStandup -= OnStandUp;
            _character.OnCancel -= OnCancel;
            _character.OnInteraction -= OnInteraction;
            _character.OnCalmDown -= OnCalmDown;
            _character.OnHit -= OnHit;
        }
    }
}
