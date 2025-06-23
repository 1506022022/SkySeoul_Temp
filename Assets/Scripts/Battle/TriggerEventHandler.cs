using UnityEngine;
using UnityEngine.Events;

public class TriggerEventHandler : MonoBehaviour
{
    public UnityEvent<Collider> OnEnter = new();
    public UnityEvent<Collider> OnStay = new();
    public UnityEvent<Collider> OnExit = new();

    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        OnStay?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke(other);
    }
}