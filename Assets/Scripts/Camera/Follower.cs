using UnityEngine;

public class Follower : MonoBehaviour
{
    private Transform _base;
    private bool _active;
    public void SetTarget(Transform target)
    {
        _base = target;
    }
    public void SetActive(bool active)
    {
        _active = active;
    }
    private void Update()
    {
        if (!_active || _base == null) return;
        transform.SetPositionAndRotation(_base.position, _base.rotation);
        transform.localScale = _base.localScale;
    }
}
