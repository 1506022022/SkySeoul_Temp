using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class CharacterJoycon
    {
        private const float STAY_INPUT = 0.55f;
        private readonly Character _character;
        private readonly CharacterController _controller;
        private readonly Stack<Func<bool>> _inputBuffer = new();
        private readonly Dictionary<int, float> _lastInputTime = new();
        private float _cooltimeOfSlide = 1f;
        private float _SlidedTime;
        public event Action<string> OnAction;
        public CharacterJoycon(Character character, CharacterController controller)
        {
            _character = character;
            _controller = controller;
            _character.OnSlideEnd += StartCooltimeAtSliding;
            _character.OnInteraction += InvokeInteraction;
            _character.OnCancel += () => _character.DoStandUp();

            SetUseMovement(out _useMovement);
            SetUseAttack(out _useAttack);
            SetUseJump(out _useJump);
            SetUseSliding(out _useSliding);
            SetUseInteraction(out _useInteraction);
            SetUseCancel(out _useCancel);
            SetUseLand(out _useLand);
        }
        private bool _useMovement;
        protected abstract void SetUseMovement(out bool useMovement);
        private bool _useAttack;
        protected abstract void SetUseAttack(out bool useAttack);
        private bool _useJump;
        protected abstract void SetUseJump(out bool useJump);
        private bool _useSliding;
        protected abstract void SetUseSliding(out bool useSliding);
        private bool _useInteraction;
        protected abstract void SetUseInteraction(out bool useInteraction);
        private bool _useCancel;
        protected abstract void SetUseCancel(out bool useCancel);
        private bool _useLand;
        protected abstract void SetUseLand(out bool useLand);
        public void UpdateInput()
        {
            if (_useMovement) UpdateMovement();
            if (_useAttack) UpdateAttack();
            if (_useJump) UpdateJump();
            if (_useSliding) UpdateSliding();
            if (_useInteraction) UpdateInteraction();
            if (_useCancel) UpdateCancel();
            if (_useLand) UpdateLand();
            InvokeInput();
        }
        private void UpdateAttack()
        {
            var attack = Input.GetButtonDown("Fire1");
            if (attack)
            {
                PushInputStack(_character.DoAttack);
            }
        }
        private void UpdateInteraction()
        {
            var input = Input.GetButtonDown("Interaction");
            if (input && InteractionSystem.TryGetInteraction(_controller.transform, out var interaction))
            {
                PushInputStack(_character.DoInteraction);
            }

        }
        private void InvokeInteraction()
        {
            if (InteractionSystem.TryGetInteraction(_controller.transform, out var interaction))
            {
                interaction.DoStart();
            }
        }
        private void UpdateJump()
        {
            var jump = Input.GetButtonDown("Jump");
            if (jump)
            {
                PushInputStack(_character.DoJump);
            }
        }
        private void UpdateCancel()
        {
            var cancle = Input.GetButtonDown("Cancel");
            if (cancle)
            {
                PushInputStack(_character.DoCancel);
            }
        }
        private void UpdateMovement()
        {
            var dir = Vector3.right * Input.GetAxisRaw("Horizontal") + Vector3.forward * Input.GetAxisRaw("Vertical");
            if (dir != Vector3.zero)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    PushInputStack(() => _character.DoRun(dir));
                    return;
                }
                PushInputStack(() => _character.DoMove(dir));
            }
        }
        private void UpdateSliding()
        {
            if (Time.time < _SlidedTime)
            {
                return;
            }
            if (_controller.minMoveDistance < _controller.velocity.magnitude &&
                Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C))
            {
                PushInputStack(() => _character.DoSlide());
            }
        }
        private void UpdateLand()
        {
            if (_controller.isGrounded)
            {
                _character.DoLand();
            }
        }
        private void InvokeInput()
        {
            var temp = new Stack<Func<bool>>();
            while (0 < _inputBuffer.Count)
            {
                var action = _inputBuffer.Pop();
                if (STAY_INPUT < Time.time - _lastInputTime[action.Method.Name.GetHashCode()])
                {
                    continue;
                }
                var actionName = action.Method.Name;
                if (action.Invoke())
                {
                    OnAction?.Invoke(actionName);
                    _inputBuffer.Clear();
                    return;
                }
                temp.Push(action);
            }
            while (0 < temp.Count)
            {
                _inputBuffer.Push(temp.Pop());
            }
        }
        private void PushInputStack(Func<bool> action)
        {
            var temp = new Stack<Func<bool>>();
            while (_inputBuffer.Count > 0)
            {
                var item = _inputBuffer.Pop();
                if (action.Method.Name == item.Method.Name)
                {
                    continue;
                }
                temp.Push(item);
            }
            while (temp.Count > 0)
            {
                _inputBuffer.Push(temp.Pop());
            }
            _inputBuffer.Push(action);
            _lastInputTime[action.Method.Name.GetHashCode()] = Time.time;
        }
        private void StartCooltimeAtSliding()
        {
            _SlidedTime = Time.time + _cooltimeOfSlide;
        }
    }
    public class HanZoomInJoycon : CharacterJoycon
    {
        public HanZoomInJoycon(Character character, CharacterController controller) : base(character, controller)
        {
        }
        protected override void SetUseAttack(out bool useAttack)
        {
            useAttack = true;
        }
        protected override void SetUseCancel(out bool useCancel)
        {
            useCancel = true;
        }
        protected override void SetUseInteraction(out bool useInteraction)
        {
            useInteraction = false;
        }
        protected override void SetUseJump(out bool useJump)
        {
            useJump = false;
        }
        protected override void SetUseLand(out bool useLand)
        {
            useLand = true;
        }
        protected override void SetUseMovement(out bool useMovement)
        {
            useMovement = true;
        }
        protected override void SetUseSliding(out bool useSliding)
        {
            useSliding = false;
        }
    }

    public class HanZoomOutJoycon : CharacterJoycon
    {
        public HanZoomOutJoycon(Character character, CharacterController controller) : base(character, controller)
        {
        }
        protected override void SetUseAttack(out bool useAttack)
        {
            useAttack = false;
        }
        protected override void SetUseCancel(out bool useCancel)
        {
            useCancel = true;
        }
        protected override void SetUseInteraction(out bool useInteraction)
        {
            useInteraction = true;
        }
        protected override void SetUseJump(out bool useJump)
        {
            useJump = true;
        }
        protected override void SetUseLand(out bool useLand)
        {
            useLand = true;
        }
        protected override void SetUseMovement(out bool useMovement)
        {
            useMovement = true;
        }
        protected override void SetUseSliding(out bool useSliding)
        {
            useSliding = true;
        }
    }
}
