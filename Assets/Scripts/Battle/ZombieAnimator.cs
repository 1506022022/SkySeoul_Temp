using UnityEngine;

namespace Battle
{
    public class ZombieAnimator : CharacterAnimator
    {
        private void DoCalmDown()
        {
            character.CalmDown();
        }
        protected override void OnAttack()
        {
            SetTrigger(STATE_ATTACK, 1);
            AwaitExitEvent(STATE_ATTACK, 1, DoCalmDown);
        }
        protected override void OnHit()
        {
            SetTrigger(STATE_HIT, 1);
            AwaitExitEvent(STATE_HIT, 1, DoCalmDown);
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
        protected override void OnIdle()
        {
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0f);
            SetTrigger(STATE_IDLE, 0);
            UpdateDirection(Vector3.zero);
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
        protected override void OnUnuse()
        {
            SetFloat(_stringToHash[PARAMETERS_SPEED], 0);
        }
    }
}
