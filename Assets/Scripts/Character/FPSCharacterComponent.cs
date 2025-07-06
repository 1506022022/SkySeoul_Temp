using Battle;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public class FPSCharacterComponent : PlayableBaseComponent
    {
        ShootingView view;
        [SerializeField] GunComponent gun;
        [SerializeField] CinemachineCamera wideCam;
        [SerializeField] CinemachineCamera zoomInCam;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            view = new ShootingView(transform, wideCam, zoomInCam);
            (gun as IInitializable)?.Initialize();
            gun.SetOwner(transform);
        }

        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            view?.UpdateView();
        }

        protected override void OnAttack(int attackType)
        {
            base.OnAttack(attackType);
            if (gun == null) return;
            gun.Fire();
        }
    }
}