using UnityEngine;

namespace Battle
{
    public class HandIK
    {
        private Transform _cross;
        private float _weight = 1f;
        private int _handLayer;
        private readonly Animator _animator;
        private const string BASE_LAYER = "Upper Boddy";
        public float ARM_LENGTH = 0.8f;
        public HandIK(Animator animator)
        {
            _animator = animator;
            SetIKLayer(BASE_LAYER);
            SetWeight(1f);
        }
        public void SetTartget(Transform target)
        {
            _cross = target;
        }
        public void SetIKLayer(string layer)
        {
            _handLayer = _animator.GetLayerIndex(layer);
        }
        public void SetWeight(float weight)
        {
            _weight = Mathf.Clamp01(weight);
        }
        public void OnAnimatorIK(int layerIndex)
        {
            if (layerIndex != _handLayer)
            {
                return;
            }
            if (_animator.IsInTransition(_handLayer))
            {
                return;
            }
            var leftArm = _animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            var rightArm = _animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            var pos1 = Vector3.Lerp(leftArm, rightArm, 0.5f);
            var distance = Vector3.Distance(pos1, _cross.position);
            var per = ARM_LENGTH / distance;
            var pos2 = Vector3.Lerp(pos1, _cross.position, per);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _weight);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _weight);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, pos2);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, pos2);
            Debug.DrawLine(leftArm, rightArm);
            Debug.DrawLine(pos1, pos2);
        }

    }
}

