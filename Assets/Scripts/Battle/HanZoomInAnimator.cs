using UnityEngine;

namespace Battle
{
    public class HanZoomInAnimator : CharacterAnimator
    {
        private Vector3 _preDir;

        private void DoCalmDown()
        {
            character.CalmDown();
        }
        protected override void OnAttack()
        {
            SetTrigger(STATE_ATTACK, 1);
            AwaitExitEvent(STATE_ATTACK, 1, DoCalmDown);
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
        protected override void OnLand()
        {
            SetTrigger(STATE_LAND, 0);
            AwaitExitEvent(STATE_LAND, 0, DoCalmDown);
        }
        protected override void OnMove(Vector3 dir)
        {
            SetTrigger(STATE_MOVE, 0);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0.5f);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0f, 1f);
            UpdateDirection(dir);
        }
        protected override void OnRun(Vector3 dir)
        {
            SetTrigger(STATE_MOVE, 0);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 1f);
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0f, 1f);
            UpdateDirection(dir);
        }
        protected override void OnStandUp()
        {
            SetBoolean(PARAMETERS_INTERACTION, false);
            SetBoolean(PARAMETERS_CROUCH, false);
        }
        private void UpdateDirection(Vector3 dir)
        {
            dir.y = 0f;
            dir.z = dir.z != 0 ? dir.z : Mathf.Abs(dir.x);
            if (_preDir == dir)
            {
                return;
            }

            SetFloat(_stringToHash[PARAMETERS_VERTICAL], dir.z);
            _preDir = dir;
        }
        protected override void OnDie()
        {
           Play(STATE_DIE);
        }
        protected override void OnUse()
        {
            animator.Play(STATE_ATTACK, 1);
        }
        protected override void OnUnuse()
        {
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0);
        }
    }
}
