using UnityEngine;
using UnityEngine.Events;

public class TriggerEventHandler : MonoBehaviour
{
    public UnityEvent<Collider> OnEnter;
    public UnityEvent<Collider> OnStay;
    public UnityEvent<Collider> OnExit;

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