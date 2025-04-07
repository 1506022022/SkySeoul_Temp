using UnityEngine;

namespace Battle
{
    public class HanZoomOutAnimator : CharacterAnimator
    {
        public HanZoomOutAnimator(Character character, Animator animator) : base(character, animator)
        {
        }
        private void DoCalmDown()
        {
            _character.CalmDown();
        }
        private void DoStandUp()
        {
            _character.DoStandUp();
        }
        protected override void OnAttack()
        {
            SetTrigger(STATE_ATTACK, 1);
            AwaitExitEvent(STATE_ATTACK, 1, DoCalmDown);
        }
        protected override void OnCalmDown()
        {
            SetTrigger(PARAMETERS_CALMDOWN, 0);
        }
        protected override void OnCancel()
        {
            SetTrigger(STATE_CANCEL, 0);
            AwaitExitEvent(STATE_CANCEL, 1, DoCalmDown);
        }
        protected override void OnFall()
        {
            SetTrigger(STATE_FALL, 0);
        }
        protected override void OnHit()
        {
            SetTrigger(STATE_HIT, 1);
            AwaitExitEvent(STATE_HIT, 1, DoCalmDown);
        }
        protected override void OnIdle()
        {
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0f);
            SetTrigger(STATE_IDLE, 0);
            UpdateDirection(Vector3.zero);
        }
        protected override void OnInteraction()
        {
            SetBoolean(PARAMETERS_INTERACTION, true);
            SetBoolean(PARAMETERS_CROUCH, true);
        }
        protected override void OnJump()
        {
            SetTrigger(STATE_JUMP, 0);
        }
        protected override void OnLand()
        {
            SetTrigger(STATE_LAND, 0);
            AwaitExitEvent(STATE_LAND, 0, DoCalmDown);
        }
        protected override void OnMove(Vector3 dir)
        {
            SetTrigger(STATE_MOVE, 0);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0.5f);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0, 1f);
            UpdateDirection(dir);
        }
        protected override void OnRun(Vector3 dir)
        {
            SetTrigger(STATE_MOVE, 0);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 1f);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0, 1f);
            UpdateDirection(dir);
        }
        protected override void OnSlide()
        {
            SetTrigger(STATE_SLIDING, 0);
            AwaitExitEvent(STATE_SLIDING, 0, DoStandUp);
        }
        protected override void OnSlideEnd()
        {
            SetTrigger(STATE_MOVE, 0);
        }
        protected override void OnStandUp()
        {
            SetBoolean(PARAMETERS_INTERACTION, false);
            SetBoolean(PARAMETERS_CROUCH, false);
        }
        private Vector3 _preDir;
        private void UpdateDirection(Vector3 dir)
        {
            if (_preDir == dir)
            {
                return;
            }

            SetFloat(_stringToHash[PARAMETERS_HORIZONTAL], dir.x);
            SetFloat(_stringToHash[PARAMETERS_VERTICAL], dir.z);
            _preDir = dir;
        }

        protected override void OnUse()
        {
            _animator.Play(STATE_IDLE, 1);
        }
        protected override void OnUnuse()
        {
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0);
        }
    }
}
