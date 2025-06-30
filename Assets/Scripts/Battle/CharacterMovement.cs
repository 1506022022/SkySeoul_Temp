using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public interface IMovement
    {
        public bool IsGrounded { get; }
        public void UpdateGravity();
    }

    public class EmptyMovement : IMovement
    {
        public bool IsGrounded => true;

        public void UpdateGravity()
        {
            
        }
    }
    public class CharacterMovement : IMovement
    {
        public float MoveSpeed = 2f;
        public float JumpPower = 4f;
        public float SlidePower = 1f;
        private readonly CharacterState character;
        private readonly CharacterController controller;
        private Coroutine slidingCoroutine;

        public bool IsGrounded => controller?.isGrounded ?? true;

        public CharacterMovement(CharacterState character, Transform transform)
        {
            this.character = character;
            if (!transform.TryGetComponent(out controller)) controller = transform.AddComponent<CharacterController>();
            AddEvent();
        }
        private void AddEvent()
        {
            character.OnMove += DoWalk;
            character.OnRun += DoRun;
            character.OnJump += DoJump;
            character.OnSlide += DoSliding;
            character.OnSlideEnd += () => CoroutineRunner.instance.StopCoroutine(slidingCoroutine);
            character.OnSlideEnd += StopVelocity;
        }
        private void SetDirOfForward(ref Vector3 dir)
        {
            var @base = controller.transform;
            dir = @base.forward * dir.z + @base.right * dir.x;
            dir.y = 0;
            dir = dir.normalized;
        }
        private void DoWalk(Vector3 dir)
        {
            SetDirOfForward(ref dir);
            controller.SimpleMove(dir * MoveSpeed);

        }
        private void DoRun(Vector3 dir)
        {
            SetDirOfForward(ref dir);
            controller.SimpleMove(dir * MoveSpeed * 1.5f);
        }
        private void DoJump()
        {
            controller.Move(Vector3.up * JumpPower);
        }
        public void UpdateGravity()
        {
            controller.Move((Physics.gravity) * Time.fixedDeltaTime);
        }
        private void DoSliding()
        {
            if (slidingCoroutine != null)
            {
                CoroutineRunner.instance.StopCoroutine(slidingCoroutine);
            }
            slidingCoroutine = CoroutineRunner.instance.StartCoroutine(LerpForward());
        }
        private IEnumerator LerpForward()
        {
            var forward = controller.velocity.normalized;
            while (true)
            {
                controller.Move(forward * Time.fixedDeltaTime * SlidePower);
                yield return new WaitForFixedUpdate();
            }
        }
        private void StopVelocity()
        {
            controller.Move(Vector3.zero);
        }
    }
}

