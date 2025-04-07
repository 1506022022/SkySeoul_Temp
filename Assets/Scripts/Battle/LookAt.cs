using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Battle
{
    public class LookAt
    {
        private readonly Transform _transform;
        public LookAt(Transform transform)
        {
            _transform = transform;
        }
        public void TurnBody(Vector3 dir)
        {
            CoroutineRunner.instance.StopCoroutine(UpdateTurnBody(dir));
            CoroutineRunner.instance.StartCoroutine(UpdateTurnBody(dir));
        }
        private IEnumerator UpdateTurnBody(Vector3 dir)
        {
            dir *= -1f;
            dir.y = 0;
            if (dir == Vector3.zero)
            {
                yield break;
            }
            var t = 0f;
            var duration = 2f;
            while (t < 1)
            {
                t = Mathf.Clamp01(t + Time.fixedDeltaTime / duration);
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, t);
                yield return new WaitForFixedUpdate();
            }
        }
    }

}
