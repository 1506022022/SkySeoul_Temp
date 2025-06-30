using Battle;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class playercontroller : IController
    {
        Dictionary<int, float> attackDuration;
        readonly IActor actor;
        float attackEndTime;
        Vector3 moveDir;
        bool isGrounded;
        bool isDead;

        public playercontroller(IActor actor)
        {
            this.actor = actor;
            attackDuration = new()
        {
            { 1, 1.067f },
            { 2, 0.367f },
            {3,0.3f }
        };
        }

        public void Update()
        {
            moveDir = Vector3.zero;
            moveDir.x = Input.GetAxisRaw("Horizontal");
            moveDir.z = Input.GetAxisRaw("Vertical");
            isGrounded = (actor as IGroundCheckable)?.IsGrounded ?? true;
            isDead = (actor as IDeathable)?.IsDead ?? false;

            if (IsInputRetry())
            {
                if (isDead)
                {
                    OnInputRetry(); return;
                }
            }

            if (IsInputJump())
            {
                if (isGrounded && IsOverdAction() && actor is IJumpable)
                {
                    attackEndTime = Time.time + attackDuration[3];
                    OnInputJump(); return;
                }
            }

            if (IsInputWalk())
            {
                if (actor is IMovable) OnInputWalk();
            }

            if (IsInputRun())
            {
                if (actor is IMovable) OnInputRun();
            }

            if (IsInputSelfHit())
            {
                if (actor is IDamageable)
                {
                    OnInputSelfHit(); return;
                }
            }

            if (IsInputEscape())
            {
                if (actor is IDeathable && !isDead)
                {
                    OnInputEscape(); return;
                }
            }

            if (IsInputSlide())
            {
                if (actor is ISliding && isGrounded)
                {
                    OnInputSlide(); return;
                }
            }

            if (IsInputMeleeAttack())
            {
                if (actor is IAttackable && isGrounded && IsOverdAction())
                {
                    OnInputMeleeAttack(); return;
                }
            }

            if (IsInputRangedAttack())
            {
                if (actor is IAttackable && isGrounded && IsOverdAction())
                {
                    OnInputRangedAttack(); return;
                }
            }
        }

        private void OnInputRangedAttack()
        {
            (actor as IAttackable)?.Attack(2);
            attackEndTime = Time.time + attackDuration[2];
        }

        private bool IsInputRangedAttack()
        {
            return isGrounded && Input.GetKeyDown(KeyCode.R);
        }

        private void OnInputMeleeAttack()
        {
            (actor as IAttackable)?.Attack(1);
            attackEndTime = Time.time + attackDuration[1];
        }

        private bool IsInputMeleeAttack()
        {
            return Input.GetKeyDown(KeyCode.E);
        }

        private bool IsOverdAction()
        {
            return attackEndTime < Time.time;
        }

        private void OnInputSlide()
        {
            (actor as ISliding)?.Slide();
        }

        private bool IsInputSlide()
        {
            return moveDir != Vector3.zero && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.C);
        }

        private void OnInputEscape()
        {
            (actor as IDeathable)?.Die();
        }

        private static bool IsInputEscape()
        {
            return Input.GetKeyDown(KeyCode.Alpha2);
        }

        private void OnInputRun()
        {
            (actor as IMovable)?.Move(moveDir, 1.5f);
        }

        private bool IsInputRun()
        {
            return isGrounded && moveDir != Vector3.zero && Input.GetKey(KeyCode.LeftShift);
        }

        private void OnInputSelfHit()
        {
            (actor as IDamageable)?.TakeDamage();
        }

        private static bool IsInputSelfHit()
        {
            return Input.GetKeyDown(KeyCode.Alpha1);
        }

        private void OnInputWalk()
        {
            (actor as IMovable)?.Move(moveDir, 1.0f);
        }

        private bool IsInputWalk()
        {
            return isGrounded && moveDir != Vector3.zero && !Input.GetKey(KeyCode.LeftShift);
        }

        private void OnInputJump()
        {
            (actor as IJumpable)?.Jump();
        }

        private bool IsInputJump()
        {
            return Input.GetAxisRaw("Jump") != 0;
        }

        private void OnInputRetry()
        {
            (actor as IDeathable).Revive();
        }

        private static bool IsInputRetry()
        {
            return Input.GetAxisRaw("Cancel") != 0;
        }
    }
}
