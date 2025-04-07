using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public class CharacterMovement
    {
        public float MoveSpeed = 2f;
        public float JumpPower = 5f;
        public float SlidPower = 1f;
        private readonly Character _character;
        private readonly CharacterController _controller;
        private Coroutine _slidingCoroutine;

        public CharacterMovement(Character character, CharacterController controller)
        {
            _character = character;
            _controller = controller;
            AddEvent();
        }
        private void AddEvent()
        {
            _character.OnMove += DoWalk;
            _character.OnRun += DoRun;
            _character.OnJump += DoJump;
            _character.OnSlide += DoSliding;
            _character.OnSlideEnd += () => CoroutineRunner.instance.StopCoroutine(_slidingCoroutine);
            _character.OnSlideEnd += StopVelocity;
        }
        private void SetDirOfForward(ref Vector3 dir)
        {
            var @base = _controller.transform;
            dir = @base.forward * dir.z + @base.right * dir.x;
            dir.y = 0;
            dir = dir.normalized;
        }
        private void DoWalk(Vector3 dir)
        {
            SetDirOfForward(ref dir);
            _controller.Move(dir * MoveSpeed * Time.fixedDeltaTime);

        }
        private void DoRun(Vector3 dir)
        {
            SetDirOfForward(ref dir);
            _controller.Move(dir * MoveSpeed * 1.5f * Time.fixedDeltaTime);
        }
        private void DoJump()
        {
            _controller.Move(Vector3.up * JumpPower);
        }
        public void UpdateGravity()
        {
            _controller.Move((Physics.gravity) * Time.fixedDeltaTime);
        }
        private void DoSliding()
        {
            if (_slidingCoroutine != null)
            {
                CoroutineRunner.instance.StopCoroutine(_slidingCoroutine);
            }
            _slidingCoroutine = CoroutineRunner.instance.StartCoroutine(LerpForward());
        }
        private IEnumerator LerpForward()
        {
            var forward = _controller.velocity.normalized;
            while (true)
            {
                _controller.Move(forward * Time.fixedDeltaTime * SlidPower);
                yield return new WaitForFixedUpdate();
            }
        }
        private void StopVelocity()
        {
            _controller.Move(Vector3.zero);
        }
    }
}

