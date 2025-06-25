using UnityEngine;

[ExecuteAlways]
public class ParticleModifier : MonoBehaviour
{
    Vector3 prePos;
    [SerializeField] Material material;


    void Update()
    {
        if (prePos != transform.position)
        {
            UpdateParticlePosition();
        }
    }

    void UpdateParticlePosition()
    {
        prePos = transform.position;
        if (material == null) return;
        material.SetVector("_Position", new Vector4(prePos.x, prePos.y, prePos.z, 0));
    }

}
