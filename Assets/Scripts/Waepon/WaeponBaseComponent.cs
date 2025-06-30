using Battle;
using Unity.VisualScripting;
using UnityEngine;

public abstract class WaeponBaseComponent : MonoBehaviour, IInitializable
{
    protected Transform owner;

    void IInitializable.Initialize()
    {
        Initialize(owner);
    }
    protected abstract void Initialize(Transform owner);

    public void SetOwner(Transform owner)
    {
        this.owner = owner;
        Initialize(owner);
    }
}

public abstract class Weapon
{
    public abstract void Fire();
}

public class Gun : Weapon
{
    Bullet bullet;

    public Gun(Transform weapon, Transform owner)
    {
        SetBullet(new Bullet(weapon, owner));
    }

    public void SetBullet(Bullet bullet)
    {
        this.bullet = bullet;
    }

    public override void Fire()
    {
        bullet.OnFire();
    }
}