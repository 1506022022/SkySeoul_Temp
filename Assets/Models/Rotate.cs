using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField, Min(0f)] float radius = 1f;
    Vector3 lastPosition;

    void Awake()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        UpdateSphereRotation();
        lastPosition = transform.position;
    }

    void UpdateSphereRotation()
    {
        var currentPosition = transform.position;
        var deltaPosition = currentPosition - lastPosition;

        if (deltaPosition.sqrMagnitude < Mathf.Epsilon) return;

        Vector3 rotationAxis = Vector3.Cross(deltaPosition.normalized, Vector3.up);

        float distance = deltaPosition.magnitude;
        float angle = (distance / radius) * Mathf.Rad2Deg;

        transform.Rotate(rotationAxis, -angle, Space.World);
    }
}
