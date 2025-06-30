using Unity.VisualScripting;
using UnityEngine;

public class GunComponent : WaeponBaseComponent
{
    Gun gun;

    protected override void Initialize(Transform owner)
    {
        gun = new(transform, owner);
    }
    public void Fire() => gun?.Fire();
}