using UnityEngine;

namespace BattlePath
{
    public interface IPathObject
    {
        public void OnUpdateTime(float normalizeTime);
    }

    public class ObjectControlPathComponent : AnimationPathComponent
    {
        [SerializeField] private GameObject _object;
        protected override void OnUpdatePath(Vector3 position)
        {
            if (_object == null)
            {
                return;
            }
            _object.transform.position = position;
        }

        protected override void OnUpdateTime(float normalizeTime)
        {
            if (_object == null || !_object.TryGetComponent<IPathObject>(out var pathObject))
            {
                return;
            }

            pathObject.OnUpdateTime(normalizeTime);
        }
    }

}
