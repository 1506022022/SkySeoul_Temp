using UnityEngine;
using UnityEngine.Events;

public class IKEventHandler : MonoBehaviour
{
    public UnityEvent<int> onAnimatorIK = new();
    private void OnAnimatorIK(int layerIndex)
    {
        onAnimatorIK?.Invoke(layerIndex);
    }
}
