#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


// Step1. Camera To Ray Debugger
// Step2. Hit Point to normal check


public class GuiDebugger : MonoBehaviour
{
    private Transform camTr;
    public float checkLength;

    // Start is called before the first frame update
    void Start()
    {
        camTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;

            Gizmos.DrawLine(camTr.position, camTr.position + camTr.forward * checkLength);
        }
    }
}
