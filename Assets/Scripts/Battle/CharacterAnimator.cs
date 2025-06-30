using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Battle
{
    public class EmptyAnimator : CharacterAnimator
    {

    }

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
        protected const string STATE_DIE = "Die";
        protected const string PARAMETERS_CALMDOWN = "CalmDown";
        protected const string PARAMETERS_SPEED = "Speed";
        protected const string PARAMETERS_CROUCH = "Crouch";
        protected const string PARAMETERS_INTERACTION = "Interaction";
        protected const string PARAMETERS_HORIZONTAL = "Horizontal";
        protected const string PARAMETERS_VERTICAL = "Vertical";
        protected Animator animator;
        protected CharacterState character;
        protected readonly Dictionary<string, int> _stringToHash = new();
        protected readonly Dictionary<int, string> _hashToString = new();
        private readonly Dictionary<int, Lerp> _lerps = new();
        private readonly Dictionary<int, Coroutine> _awaits = new();
        private string _preTrigger = STATE_IDLE;
        private static readonly CoroutineRunner _runner = CoroutineRunner.instance;
        protected virtual void OnUse() { }
        protected virtual void OnUnuse() { }
        protected virtual void OnIdle() { }
        protected virtual void OnMove(Vector3 dir) { }
        protected virtual void OnRun(Vector3 dir) { }
        protected virtual void OnAttack() { }
        protected virtual void OnJump() { }
        protected virtual void OnFall() { }
        protected virtual void OnLand() { }
        protected virtual void OnSlide() { }
        protected virtual void OnSlideEnd() { }
        protected virtual void OnStandUp() { }
        protected virtual void OnCancel() { }
        protected virtual void OnInteraction() { }
        protected virtual void OnCalmDown() { }
        protected virtual void OnHit() { }
        protected virtual void OnDie()
        {
        
        }
        public void Initialize(CharacterState character, Animator animator)
        {
            this.character = character;
            this.animator = animator;

            _stringToHash.Add(STATE_IDLE, Animator.StringToHash(STATE_IDLE));
            _stringToHash.Add(STATE_MOVE, Animator.StringToHash(STATE_MOVE));
            _stringToHash.Add(STATE_ATTACK, Animator.StringToHash(STATE_ATTACK));
            _stringToHash.Add(STATE_JUMP, Animator.StringToHash(STATE_JUMP));
            _stringToHash.Add(STATE_LAND, Animator.StringToHash(STATE_LAND));
            _stringToHash.Add(STATE_HIT, Animator.StringToHash(STATE_HIT));
            _stringToHash.Add(STATE_CANCEL, Animator.StringToHash(STATE_CANCEL));
            _stringToHash.Add(STATE_FALL, Animator.StringToHash(STATE_FALL));
            _stringToHash.Add(STATE_SLIDING, Animator.StringToHash(STATE_SLIDING));
            _stringToHash.Add(STATE_DIE, Animator.StringToHash(STATE_DIE));
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

        protected void Play(string name)
        {
            animator.Play(name);
        }
        protected void SetFloat(int parameter, float value, float duration = 0f)
        {
            GetLerp(parameter).SetDuration(duration)
                              .SetFloat(_hashToString[parameter], value);
        }
        protected void SetTrigger(string trigger, int layer)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            var state = stateInfo.shortNameHash;
            var loop = stateInfo.loop;
            if (!loop && _preTrigger == trigger && state == _stringToHash[trigger])
            {
                animator.Play(state, layer, 0f);
            }
            else
            {
                animator.ResetTrigger(_preTrigger);
                animator.SetTrigger(trigger);
            }
            _preTrigger = trigger;
        }
        protected void SetBoolean(string boolean, bool value)
        {
            animator.SetBool(boolean, value);
        }
        protected void AwaitExitEvent(string exitState, int layer, UnityAction exitEvent)
        {
            if (_awaits.TryGetValue(_stringToHash[exitState], out var await))
            {
                _runner.StopCoroutine(await);
            }

            _awaits[_stringToHash[exitState]] = _runner.StartCoroutine(InvokeEixitEvent(_stringToHash[exitState], layer, exitEvent));
        }
        IEnumerator InvokeEixitEvent(int exitState, int layer, UnityAction exitEvent)
        {
            yield return new WaitUntil(() => IsSame(exitState, layer));
            yield return new WaitUntil(() => IsExit(exitState, layer));
            exitEvent?.Invoke();
        }
        bool IsSame(int exitState, int layer)
        {
            if (animator == null) return true;
            var state = animator.GetCurrentAnimatorStateInfo(layer);
            return state.shortNameHash == exitState;
        }
        bool IsExit(int exitState, int layer)
        {
            if (animator == null) return true;
            var state = animator.GetCurrentAnimatorStateInfo(layer);
            var outState = state.shortNameHash != exitState;
            var outRange = 1 <= state.normalizedTime;
            return outState || outRange;
        }
        Lerp GetLerp(int parameter)
        {
            if (!_lerps.TryGetValue(parameter, out var value))
            {
                value = new Lerp();
                value.SetAnimator(animator);
                _lerps.Add(parameter, value);
            }
            return value;
        }
        void AddCharacterEvnet()
        {
            character.OnIdle += OnIdle;
            character.OnMove += OnMove;
            character.OnRun += OnRun;
            character.OnAttack += OnAttack;
            character.OnJump += OnJump;
            character.OnFall += OnFall;
            character.OnLand += OnLand;
            character.OnSlide += OnSlide;
            character.OnSlideEnd += OnSlideEnd;
            character.OnStandup += OnStandUp;
            character.OnCancel += OnCancel;
            character.OnInteraction += OnInteraction;
            character.OnCalmDown += OnCalmDown;
            character.OnHit += OnHit;
            character.OnDead += OnDie;
        }
        void RemoveCharacterEvent()
        {
            character.OnIdle -= OnIdle;
            character.OnMove -= OnMove;
            character.OnRun -= OnRun;
            character.OnAttack -= OnAttack;
            character.OnJump -= OnJump;
            character.OnFall -= OnFall;
            character.OnLand -= OnLand;
            character.OnSlide -= OnSlide;
            character.OnSlideEnd -= OnSlideEnd;
            character.OnStandup -= OnStandUp;
            character.OnCancel -= OnCancel;
            character.OnInteraction -= OnInteraction;
            character.OnCalmDown -= OnCalmDown;
            character.OnHit -= OnHit;
            character.OnDead -= OnDie;
        }
    }
}
